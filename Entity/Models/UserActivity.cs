using Entity.Base;

namespace Entity.Models;

public class UserActivity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty; // Login, Logout, API_Call, Page_View, vb.
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Endpoint { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public AppUser User { get; set; } = null!;
} 