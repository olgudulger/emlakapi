using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Abstract;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserActivityController : ControllerBase
{
    private readonly IUserActivityService _userActivityService;

    public UserActivityController(IUserActivityService userActivityService)
    {
        _userActivityService = userActivityService;
    }

    /// <summary>
    /// Kullanıcının kendi aktivitelerini getirir
    /// </summary>
    [HttpGet("my-activities")]
    public async Task<IActionResult> GetMyActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            var activities = await _userActivityService.GetUserActivitiesAsync(userId, page, pageSize);
            
            return Ok(new { 
                success = true, 
                data = activities.Select(a => new {
                    a.Id,
                    a.ActivityType,
                    a.Description,
                    a.IpAddress,
                    a.Endpoint,
                    a.ActivityDate
                }),
                page,
                pageSize
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Aktiviteler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Online kullanıcıları getirir (Admin)
    /// </summary>
    [HttpGet("online-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOnlineUsers()
    {
        try
        {
            var onlineUsers = await _userActivityService.GetOnlineUsersAsync();
            
            return Ok(new { 
                success = true, 
                data = onlineUsers.Select(u => new {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Role,
                    u.LastLoginDate,
                    u.LastActivityDate,
                    u.LastLoginIp,
                    u.IsOnline
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Online kullanıcılar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Son aktif kullanıcıları getirir (Admin)
    /// </summary>
    [HttpGet("recently-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRecentlyActiveUsers([FromQuery] int minutes = 30)
    {
        try
        {
            var activeUsers = await _userActivityService.GetRecentlyActiveUsersAsync(minutes);
            
            return Ok(new { 
                success = true, 
                data = activeUsers.Select(u => new {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Role,
                    u.LastLoginDate,
                    u.LastActivityDate,
                    u.LastLoginIp,
                    u.IsOnline,
                    MinutesAgo = u.LastActivityDate.HasValue ? 
                        (int?)(DateTime.UtcNow - u.LastActivityDate.Value).TotalMinutes : null
                }),
                minutes
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Aktif kullanıcılar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcı aktivite istatistiklerini getirir (Admin)
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActivityStats()
    {
        try
        {
            var stats = await _userActivityService.GetUserActivityStatsAsync();
            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İstatistikler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirli bir kullanıcının aktivitelerini getirir (Admin)
    /// </summary>
    [HttpGet("user/{userId}/activities")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserActivities(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var activities = await _userActivityService.GetUserActivitiesAsync(userId, page, pageSize);
            
            return Ok(new { 
                success = true, 
                data = activities.Select(a => new {
                    a.Id,
                    a.UserId,
                    a.ActivityType,
                    a.Description,
                    a.IpAddress,
                    a.UserAgent,
                    a.Endpoint,
                    a.ActivityDate
                }),
                userId,
                page,
                pageSize
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı aktiviteleri getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcıyı offline yapar (Admin)
    /// </summary>
    [HttpPost("user/{userId}/set-offline")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUserOffline(string userId)
    {
        try
        {
            await _userActivityService.SetUserOfflineAsync(userId);
            return Ok(new { success = true, message = "Kullanıcı offline yapıldı" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İşlem başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Aktivite günceller (heartbeat için)
    /// </summary>
    [HttpPost("heartbeat")]
    public async Task<IActionResult> UpdateActivity()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            await _userActivityService.UpdateUserActivityAsync(userId);
            return Ok(new { success = true, message = "Aktivite güncellendi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Aktivite güncellenemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcıyı manuel olarak offline yapar
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> ManualLogout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            await _userActivityService.SetUserOfflineAsync(userId);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Çıkış işlemi başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Pasif kullanıcıları kontrol eder ve offline yapar (Admin)
    /// </summary>
    [HttpPost("cleanup-inactive")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupInactiveUsers([FromQuery] int minutes = 30)
    {
        try
        {
            await _userActivityService.CheckAndSetInactiveUsersOfflineAsync(minutes);
            return Ok(new { success = true, message = $"{minutes} dakikadan fazla pasif olan kullanıcılar offline yapıldı" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Cleanup işlemi başarısız: {ex.Message}" });
        }
    }
} 