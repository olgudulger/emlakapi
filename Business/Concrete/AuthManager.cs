using Business.Abstract;
using Business.DTOs.Auth;
using Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Abstract;
using System.Linq;

namespace Business.Concrete;

public class AuthManager : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserActivityService _userActivityService;

    public AuthManager(
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IUserActivityService userActivityService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _userActivityService = userActivityService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string? ipAddress = null)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Kullanıcı adı veya şifre hatalı"
            };
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Kullanıcı adı veya şifre hatalı"
            };
        }

        // User activity tracking - Login bilgilerini güncelle
        if (!string.IsNullOrEmpty(ipAddress))
        {
            await _userActivityService.UpdateUserLoginAsync(user.Id, ipAddress);
        }

        var token = await GenerateJwtTokenAsync(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Giriş başarılı",
            Token = token.Token,
            RefreshToken = refreshToken,
            ExpiresAt = token.ExpiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Role = user.Role
            }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
        if (existingUser != null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Bu kullanıcı adı zaten kullanılıyor"
            };
        }

        var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingEmail != null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Bu e-posta adresi zaten kullanılıyor"
            };
        }

        var user = new AppUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            Role = "User"
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        await _userManager.AddToRoleAsync(user, "User");

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Kayıt başarılı"
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _unitOfWork.RefreshTokens.GetByIdAsync(1); // Temporary fix - will be replaced with proper method
        
        if (storedToken == null || !storedToken.IsActive)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Geçersiz refresh token"
            };
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Kullanıcı bulunamadı"
            };
        }

        // Eski token'ı revoke et
        storedToken.RevokedAt = DateTime.UtcNow;
        // _unitOfWork.RefreshTokens.Update(storedToken); // Will be fixed with proper repository

        // Yeni token'lar oluştur
        var newToken = await GenerateJwtTokenAsync(user);
        var newRefreshToken = await GenerateRefreshTokenAsync(user);

        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Token yenilendi",
            Token = newToken.Token,
            RefreshToken = newRefreshToken,
            ExpiresAt = newToken.ExpiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Role = user.Role
            }
        };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        try
        {
            // Kullanıcıyı offline yap
            await _userActivityService.SetUserOfflineAsync(userId);
            
            // Kullanıcının tüm aktif refresh token'larını revoke et
            var userRefreshTokens = await _unitOfWork.RefreshTokens.GetAllAsync();
            var userActiveTokens = userRefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToList();

            foreach (var token in userActiveTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = "LOGOUT";
                // _unitOfWork.RefreshTokens.Update(token); // Güncelleme işlemi repository'de implement edildiğinde aktif olacak
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            // Logout aktivitesini logla
            await _userActivityService.LogActivityAsync(userId, "LOGOUT", "Kullanıcı manuel olarak çıkış yaptı");
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        // Will be implemented with proper repository methods
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Kullanıcı bulunamadı"
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Şifre başarıyla değiştirildi"
        };
    }

    public async Task<bool> VerifyTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<(string Token, DateTime ExpiresAt)> GenerateJwtTokenAsync(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]));

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, user.Role)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiresAt);
    }

    private async Task<string> GenerateRefreshTokenAsync(AppUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        return refreshToken.Token;
    }
} 