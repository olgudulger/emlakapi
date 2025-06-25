using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Location;

public class DistrictUpdateDto
{
    [Required(ErrorMessage = "İlçe adı gereklidir")]
    [StringLength(100, ErrorMessage = "İlçe adı en fazla 100 karakter olabilir")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "İl ID gereklidir")]
    [Range(1, int.MaxValue, ErrorMessage = "İl ID geçerli bir değer olmalıdır")]
    public int ProvinceId { get; set; }
} 