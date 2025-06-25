using Entity.Models;

namespace Business.Abstract;

public interface IUserActivityService
{
    Task LogActivityAsync(string userId, string activityType, string? description = null, 
        string? ipAddress = null, string? userAgent = null, string? endpoint = null);
    
    Task UpdateUserLoginAsync(string userId, string ipAddress);
    Task UpdateUserActivityAsync(string userId);
    Task SetUserOfflineAsync(string userId);
    Task SetUserOnlineAsync(string userId, string? connectionId = null);
    
    Task<List<UserActivity>> GetUserActivitiesAsync(string userId, int page = 1, int pageSize = 20);
    Task<List<AppUser>> GetOnlineUsersAsync();
    Task<List<AppUser>> GetRecentlyActiveUsersAsync(int minutes = 30);
    Task<Dictionary<string, object>> GetUserActivityStatsAsync();
    Task CheckAndSetInactiveUsersOfflineAsync(int inactiveMinutes = 30);
    Task SetAllUsersOfflineAsync();
} 