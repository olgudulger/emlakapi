using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.User;

public class AdminPasswordChangeDto
{
    [Required(ErrorMessage = "Yeni şifre gereklidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
        ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir")]
    public required string NewPassword { get; set; }
    
    [Required(ErrorMessage = "Şifre onayı gereklidir")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
    public required string ConfirmPassword { get; set; }
} 