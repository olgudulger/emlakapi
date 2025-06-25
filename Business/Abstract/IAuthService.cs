using Business.DTOs.Auth;

namespace Business.Abstract;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string? ipAddress = null);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<AuthResponseDto> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> VerifyTokenAsync(string token);
} 