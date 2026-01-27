namespace HoaVanEcommerce.BE.Application.DTOs.Auth;

public class AuthResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string Token { get; set; } = null!;
}

