using Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Customer;

public class CustomerCreateDto
{
    [Required(ErrorMessage = "Müşteri adı gereklidir")]
    [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon numarası gereklidir")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bütçe gereklidir")]
    [Range(0, double.MaxValue, ErrorMessage = "Bütçe 0'dan büyük olmalıdır")]
    public decimal Budget { get; set; }

    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "İlgi türü gereklidir")]
    public InterestType InterestType { get; set; }

    [Required(ErrorMessage = "Müşteri türü gereklidir")]
    public CustomerType CustomerType { get; set; } = CustomerType.AlıcıSatıcı;

    /// <summary>
    /// Tercih edilen iller ve her il için tercih edilen ilçeler
    /// </summary>
    public List<ProvincePreferenceDto> ProvincePreferences { get; set; } = new();

    // Geriye uyumluluk için - deprecated
    [Obsolete("ProvincePreferences kullanın")]
    public List<int> ProvinceIds { get; set; } = new();
} 