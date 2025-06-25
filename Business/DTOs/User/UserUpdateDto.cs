using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.User;

public class UserUpdateDto
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
    public required string Username { get; set; }
    
    [Required(ErrorMessage = "E-posta gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public required string Email { get; set; }
    
    [StringLength(50, ErrorMessage = "Rol en fazla 50 karakter olabilir")]
    public string? Role { get; set; }
} 