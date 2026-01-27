using HoaVanEcommerce.BE.Application.DTOs.Auth;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}

