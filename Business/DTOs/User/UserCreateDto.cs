using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.User;

public class UserCreateDto
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
    public required string Username { get; set; }
    
    [Required(ErrorMessage = "E-posta gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Şifre gereklidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
        ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir")]
    public required string Password { get; set; }
    
    [Required(ErrorMessage = "Şifre onayı gereklidir")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public required string ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "Rol gereklidir")]
    [StringLength(50, ErrorMessage = "Rol en fazla 50 karakter olabilir")]
    public required string Role { get; set; }
} 