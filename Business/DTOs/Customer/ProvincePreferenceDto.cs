using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Customer;

public class ProvincePreferenceDto
{
    [Required(ErrorMessage = "İl ID'si gereklidir")]
    public int ProvinceId { get; set; }
    
    public List<int> DistrictIds { get; set; } = new();
} 