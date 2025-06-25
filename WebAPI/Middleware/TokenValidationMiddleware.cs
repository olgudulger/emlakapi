using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Business.Abstract;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Identity;
using Entity.Models;

namespace WebAPI.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, 
        UserManager<AppUser> userManager,
        IUserActivityService userActivityService,
        IUnitOfWork unitOfWork)
    {
        try
        {
            // Authorization header'ını kontrol et
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                await ValidateTokenAndUpdateUserStatus(token, context, userManager, userActivityService, unitOfWork);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation middleware hatası: {Message}", ex.Message);
        }

        await _next(context);
    }

    private async Task ValidateTokenAndUpdateUserStatus(
        string token, 
        HttpContext context,
        UserManager<AppUser> userManager,
        IUserActivityService userActivityService,
        IUnitOfWork unitOfWork)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            if (!tokenHandler.CanReadToken(token))
                return;

            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Token expire olmuş mu kontrol et
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await HandleExpiredToken(jwtToken, userManager, userActivityService, context);
                return;
            }

            // Token geçerli ise kullanıcı aktivitesini güncelle
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await HandleValidToken(userId, userManager, userActivityService, context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation sırasında hata: {Message}", ex.Message);
        }
    }

    private async Task HandleExpiredToken(
        JwtSecurityToken jwtToken,
        UserManager<AppUser> userManager,
        IUserActivityService userActivityService,
        HttpContext context)
    {
        try
        {
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var user = await userManager.FindByIdAsync(userId);
            if (user != null && user.IsOnline)
            {
                // Kullanıcıyı offline yap
                await userActivityService.SetUserOfflineAsync(userId);
                
                await userActivityService.LogActivityAsync(
                    userId, 
                    "TOKEN_EXPIRED", 
                    "JWT token süresi dolduğu için otomatik çıkış yapıldı", 
                    context.Connection.RemoteIpAddress?.ToString()
                );

                _logger.LogInformation("Kullanıcı {UserId} expired token sebebiyle offline yapıldı", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Expired token işlemi sırasında hata: {Message}", ex.Message);
        }
    }

    private async Task HandleValidToken(
        string userId,
        UserManager<AppUser> userManager,
        IUserActivityService userActivityService,
        HttpContext context)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Kullanıcı offline ise online yap
                if (!user.IsOnline)
                {
                    await userActivityService.SetUserOnlineAsync(userId);
                    
                    await userActivityService.LogActivityAsync(
                        userId, 
                        "AUTO_ONLINE", 
                        "Geçerli token ile otomatik online yapıldı", 
                        context.Connection.RemoteIpAddress?.ToString()
                    );

                    _logger.LogInformation("Kullanıcı {UserId} ({Username}) otomatik online yapıldı", userId, user.UserName);
                }
                else
                {
                    // Aktiviteyi güncelle
                    await userActivityService.UpdateUserActivityAsync(userId);
                    _logger.LogDebug("Kullanıcı {UserId} aktivitesi güncellendi", userId);
                }
            }
            else
            {
                _logger.LogWarning("Valid token ile kullanıcı bulunamadı: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Valid token işlemi sırasında hata: {Message}", ex.Message);
        }
    }
} 