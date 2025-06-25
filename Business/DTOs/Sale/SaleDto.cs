using Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Sale;

/// <summary>
/// Satış oluşturma DTO'su
/// </summary>
public class SaleCreateDto
{
    [Required(ErrorMessage = "Emlak seçimi zorunludur")]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Alıcı müşteri seçimi zorunludur")]
    public int BuyerCustomerId { get; set; }

    [Required(ErrorMessage = "Satış fiyatı zorunludur")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Satış fiyatı 0'dan büyük olmalıdır")]
    public decimal SalePrice { get; set; }

    [Required(ErrorMessage = "Komisyon tutarı zorunludur")]
    [Range(0, double.MaxValue, ErrorMessage = "Komisyon tutarı negatif olamaz")]
    public decimal Commission { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Masraf tutarı negatif olamaz")]
    public decimal Expenses { get; set; } = 0;

    [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır")]
    public decimal? CommissionRate { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
    public string? Notes { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Completed;
}

/// <summary>
/// Satış güncelleme DTO'su
/// </summary>
public class SaleUpdateDto
{
    [Required(ErrorMessage = "Alıcı müşteri seçimi zorunludur")]
    public int BuyerCustomerId { get; set; }

    [Required(ErrorMessage = "Satış fiyatı zorunludur")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Satış fiyatı 0'dan büyük olmalıdır")]
    public decimal SalePrice { get; set; }

    [Required(ErrorMessage = "Komisyon tutarı zorunludur")]
    [Range(0, double.MaxValue, ErrorMessage = "Komisyon tutarı negatif olamaz")]
    public decimal Commission { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Masraf tutarı negatif olamaz")]
    public decimal Expenses { get; set; } = 0;

    [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır")]
    public decimal? CommissionRate { get; set; }

    public DateTime SaleDate { get; set; }

    [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
    public string? Notes { get; set; }

    public SaleStatus Status { get; set; }
}

/// <summary>
/// Satış listesi DTO'su
/// </summary>
public class SaleListDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string SellerCustomerName { get; set; } = string.Empty;
    public string BuyerCustomerName { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public decimal Commission { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal? CommissionRate { get; set; }
    public DateTime SaleDate { get; set; }
    public SaleStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Satış detay DTO'su
/// </summary>
public class SaleDetailDto
{
    public int Id { get; set; }
    
    // Property Bilgileri
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string PropertyLocation { get; set; } = string.Empty;
    
    // Satıcı Müşteri Bilgileri
    public int SellerCustomerId { get; set; }
    public string SellerCustomerName { get; set; } = string.Empty;
    public string SellerCustomerPhone { get; set; } = string.Empty;
    
    // Alıcı Müşteri Bilgileri
    public int BuyerCustomerId { get; set; }
    public string BuyerCustomerName { get; set; } = string.Empty;
    public string BuyerCustomerPhone { get; set; } = string.Empty;
    
    // Satış Bilgileri
    public decimal SalePrice { get; set; }
    public decimal Commission { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal? CommissionRate { get; set; }
    public DateTime SaleDate { get; set; }
    public string? Notes { get; set; }
    public SaleStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    
    // Kullanıcı Bilgileri
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    
    // Zaman Bilgileri
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Satış istatistikleri DTO'su
/// </summary>
public class SaleStatisticsDto
{
    public int TotalSalesCount { get; set; }
    public decimal TotalSalesAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalNetProfit { get; set; }
    public decimal AverageSalePrice { get; set; }
    public decimal AverageCommission { get; set; }
    public decimal AverageNetProfit { get; set; }
} 