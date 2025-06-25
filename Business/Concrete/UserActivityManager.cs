using Business.Abstract;
using Entity.Models;
using Microsoft.AspNetCore.Identity;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete;

public class UserActivityManager : IUserActivityService
{
    private readonly EmlakDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserActivityManager(EmlakDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task LogActivityAsync(string userId, string activityType, string? description = null, 
        string? ipAddress = null, string? userAgent = null, string? endpoint = null)
    {
        var activity = new UserActivity
        {
            UserId = userId,
            ActivityType = activityType,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Endpoint = endpoint,
            ActivityDate = DateTime.UtcNow
        };

        _context.UserActivities.Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserLoginAsync(string userId, string ipAddress)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            user.LastActivityDate = DateTime.UtcNow;
            user.LastLoginIp = ipAddress;
            user.IsOnline = true;
            await _userManager.UpdateAsync(user);

            await LogActivityAsync(userId, "LOGIN", "Kullanıcı giriş yaptı", ipAddress);
        }
    }

    public async Task UpdateUserActivityAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.LastActivityDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task SetUserOfflineAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.IsOnline = false;
            user.CurrentConnectionId = null;
            await _userManager.UpdateAsync(user);

            await LogActivityAsync(userId, "LOGOUT", "Kullanıcı çıkış yaptı");
        }
    }

    public async Task SetUserOnlineAsync(string userId, string? connectionId = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.IsOnline = true;
            user.CurrentConnectionId = connectionId;
            user.LastActivityDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<List<UserActivity>> GetUserActivitiesAsync(string userId, int page = 1, int pageSize = 20)
    {
        return await _context.UserActivities
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.ActivityDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<AppUser>> GetOnlineUsersAsync()
    {
        return await _userManager.Users
            .Where(u => u.IsOnline)
            .OrderByDescending(u => u.LastActivityDate)
            .ToListAsync();
    }

    public async Task<List<AppUser>> GetRecentlyActiveUsersAsync(int minutes = 30)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
        return await _userManager.Users
            .Where(u => u.LastActivityDate >= cutoffTime)
            .OrderByDescending(u => u.LastActivityDate)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetUserActivityStatsAsync()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var onlineUsers = await _userManager.Users.CountAsync(u => u.IsOnline);
        var activeToday = await _userManager.Users.CountAsync(u => u.LastActivityDate >= DateTime.UtcNow.Date);
        var activeThisWeek = await _userManager.Users.CountAsync(u => u.LastActivityDate >= DateTime.UtcNow.AddDays(-7));
        
        var recentActivities = await _context.UserActivities
            .Where(ua => ua.ActivityDate >= DateTime.UtcNow.AddHours(-24))
            .GroupBy(ua => ua.ActivityType)
            .Select(g => new { ActivityType = g.Key, Count = g.Count() })
            .ToListAsync();

        return new Dictionary<string, object>
        {
            { "TotalUsers", totalUsers },
            { "OnlineUsers", onlineUsers },
            { "ActiveToday", activeToday },
            { "ActiveThisWeek", activeThisWeek },
            { "RecentActivities", recentActivities },
            { "LastUpdated", DateTime.UtcNow }
        };
    }

    public async Task CheckAndSetInactiveUsersOfflineAsync(int inactiveMinutes = 30)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-inactiveMinutes);
        
        var inactiveUsers = await _userManager.Users
            .Where(u => u.IsOnline && u.LastActivityDate < cutoffTime)
            .ToListAsync();

        foreach (var user in inactiveUsers)
        {
            user.IsOnline = false;
            user.CurrentConnectionId = null;
            await _userManager.UpdateAsync(user);
            
            await LogActivityAsync(user.Id, "AUTO_LOGOUT", 
                $"Kullanıcı {inactiveMinutes} dakika pasiflik sebebiyle otomatik çıkış yapıldı");
        }
    }

    public async Task SetAllUsersOfflineAsync()
    {
        var onlineUsers = await _userManager.Users
            .Where(u => u.IsOnline)
            .ToListAsync();

        foreach (var user in onlineUsers)
        {
            user.IsOnline = false;
            user.CurrentConnectionId = null;
            await _userManager.UpdateAsync(user);
            
            await LogActivityAsync(user.Id, "SYSTEM_LOGOUT", "Sistem restart/shutdown sebebiyle çıkış");
        }
    }
} 