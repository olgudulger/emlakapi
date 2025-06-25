using Microsoft.AspNetCore.Identity;

namespace Entity.Models
{
    public class AppUser : IdentityUser
    {
        // Ek alanlar ekleyebilirsin (ad, soyad vs.)
        public string Role { get; set; } = string.Empty;
        
        // Aktif kullanıcı yönetimi için yeni alanlar
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool IsOnline { get; set; } = false;
        public string? LastLoginIp { get; set; }
        public string? CurrentConnectionId { get; set; } // SignalR için
    }
}
