using HoaVanEcommerce.BE.Application.DTOs.Auth;
using HoaVanEcommerce.BE.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_ALREADY_EXISTS")
        {
            return BadRequest(new { message = "Email đã tồn tại." });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "INVALID_CREDENTIALS")
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "USER_INACTIVE")
        {
            return Unauthorized(new { message = "Tài khoản đã bị khóa." });
        }
    }
}

