using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Location;

public class NeighborhoodCreateDto
{
    [Required(ErrorMessage = "Mahalle adı gereklidir")]
    [StringLength(100, ErrorMessage = "Mahalle adı en fazla 100 karakter olabilir")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "İlçe ID gereklidir")]
    [Range(1, int.MaxValue, ErrorMessage = "İlçe ID geçerli bir değer olmalıdır")]
    public int DistrictId { get; set; }
} 