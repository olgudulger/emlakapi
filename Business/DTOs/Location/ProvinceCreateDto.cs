using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Location;

public class ProvinceCreateDto
{
    [Required(ErrorMessage = "İl adı gereklidir")]
    [StringLength(100, ErrorMessage = "İl adı en fazla 100 karakter olabilir")]
    public required string Name { get; set; }
} 