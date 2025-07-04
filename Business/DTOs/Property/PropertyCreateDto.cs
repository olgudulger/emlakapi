using Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Property;

public class PropertyCreateDto
{
    [Required(ErrorMessage = "Emlak türü gereklidir")]
    public PropertyType PropertyType { get; set; }

    [Required(ErrorMessage = "Başlık gereklidir")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İl gereklidir")]
    public int ProvinceId { get; set; }

    [Required(ErrorMessage = "İlçe gereklidir")]
    public int DistrictId { get; set; }

    [Required(ErrorMessage = "Mahalle gereklidir")]
    public int NeighborhoodId { get; set; }

    [Required(ErrorMessage = "Aracı adı gereklidir")]
    [StringLength(100, ErrorMessage = "Aracı adı en fazla 100 karakter olabilir")]
    public string IntermediaryFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Aracı telefonu gereklidir")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string IntermediaryPhone { get; set; } = string.Empty;

    public PropertyStatus Status { get; set; } = PropertyStatus.Satilik;

    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Müşteri seçimi gereklidir")]
    public int CustomerId { get; set; }

    // Type specific properties stored as JSON
    public Dictionary<string, object> TypeSpecificProperties { get; set; } = new();
} 