using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre gereklidir")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
} 