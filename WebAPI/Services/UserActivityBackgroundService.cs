using Business.Abstract;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entity.Models;

namespace WebAPI.Services;

public class UserActivityBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserActivityBackgroundService> _logger;

    public UserActivityBackgroundService(
        IServiceScopeFactory scopeFactory, 
        ILogger<UserActivityBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var userActivityService = scope.ServiceProvider.GetRequiredService<IUserActivityService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                _logger.LogInformation("UserActivityBackgroundService başlıyor - Online kullanıcı temizliği");

                // Önce online kullanıcı sayısını logla
                var onlineCountBefore = await userManager.Users.CountAsync(u => u.IsOnline);
                _logger.LogInformation("Temizlik öncesi online kullanıcı sayısı: {Count}", onlineCountBefore);

                // 1. Pasif kullanıcıları offline yap (60 dakika - daha toleranslı yaptık)
                await userActivityService.CheckAndSetInactiveUsersOfflineAsync(60);
                
                // 2. Refresh token'ı expired olan kullanıcıları offline yap
                await CheckExpiredRefreshTokensAndSetOffline(unitOfWork, userManager, userActivityService);
                
                // Sonraki online kullanıcı sayısını logla
                var onlineCountAfter = await userManager.Users.CountAsync(u => u.IsOnline);
                _logger.LogInformation("Temizlik sonrası online kullanıcı sayısı: {Count}", onlineCountAfter);

                _logger.LogInformation("Kullanıcı durumu temizleme işlemi tamamlandı: {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserActivityBackgroundService hatası: {Message}", ex.Message);
            }

            // Her 15 dakikada bir kontrol et (daha az sıklık)
            _logger.LogInformation("Background service 15 dakika bekliyor...");
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

    /// <summary>
    /// Refresh token'ı expired olan online kullanıcıları offline yapar
    /// </summary>
    private async Task CheckExpiredRefreshTokensAndSetOffline(
        IUnitOfWork unitOfWork, 
        UserManager<AppUser> userManager,
        IUserActivityService userActivityService)
    {
        try
        {
            // Tüm refresh token'ları al
            var allRefreshTokens = await unitOfWork.RefreshTokens.GetAllAsync();
            
            // Online olan kullanıcıları al
            var onlineUsers = await userManager.Users
                .Where(u => u.IsOnline)
                .ToListAsync();

            foreach (var user in onlineUsers)
            {
                // Kullanıcının aktif refresh token'ı var mı kontrol et
                var userActiveTokens = allRefreshTokens
                    .Where(rt => rt.UserId == user.Id && rt.IsActive)
                    .ToList();

                // Aktif token yoksa offline yap
                if (!userActiveTokens.Any())
                {
                    await userActivityService.SetUserOfflineAsync(user.Id);
                    
                    await userActivityService.LogActivityAsync(
                        user.Id, 
                        "TOKEN_CLEANUP", 
                        "Aktif refresh token bulunamadığı için otomatik çıkış yapıldı"
                    );

                    _logger.LogInformation(
                        "Kullanıcı {UserId} ({Username}) aktif refresh token'ı olmadığı için offline yapıldı", 
                        user.Id, user.UserName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh token cleanup işlemi sırasında hata: {Message}", ex.Message);
        }
    }
} 