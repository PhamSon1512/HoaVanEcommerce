using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HoaVanEcommerce.BE.Application.DTOs.Auth;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HoaVanEcommerce.BE.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
        {
            throw new InvalidOperationException("EMAIL_ALREADY_EXISTS");
        }

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var customerRole = await _roleRepository.GetByNameAsync("Customer");
        if (customerRole is not null)
        {
            await _roleRepository.AddUserRoleAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id
            });
            await _roleRepository.SaveChangesAsync();
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("INVALID_CREDENTIALS");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("INVALID_CREDENTIALS");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("USER_INACTIVE");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? "Temporary_Dev_Key_Change_Me";
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "HoaVanEcommerce";
        var audience = jwtSection.GetValue<string>("Audience") ?? "HoaVanEcommerceClient";
        var expiresMinutes = jwtSection.GetValue<int?>("ExpiresMinutes") ?? 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("fullName", user.FullName ?? string.Empty)
        };

        var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

