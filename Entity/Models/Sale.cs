using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class Sale : BaseEntity
{
    /// <summary>
    /// Satılan emlak ID'si
    /// </summary>
    public int PropertyId { get; set; }
    
    /// <summary>
    /// Satılan emlak (Navigation Property)
    /// </summary>
    public Property Property { get; set; } = null!;
    
    /// <summary>
    /// Alıcı müşteri ID'si
    /// </summary>
    public int BuyerCustomerId { get; set; }
    
    /// <summary>
    /// Alıcı müşteri (Navigation Property)
    /// </summary>
    public Customer BuyerCustomer { get; set; } = null!;
    
    /// <summary>
    /// Satış fiyatı
    /// </summary>
    public decimal SalePrice { get; set; }
    
    /// <summary>
    /// Alınan komisyon tutarı
    /// </summary>
    public decimal Commission { get; set; }
    
    /// <summary>
    /// Masraf tutarı (varsa)
    /// </summary>
    public decimal Expenses { get; set; } = 0;
    
    /// <summary>
    /// Net kazanç (Commission - Expenses) - Calculated property
    /// </summary>
    public decimal NetProfit => Commission - Expenses;
    
    /// <summary>
    /// Komisyon oranı (%) - İsteğe bağlı bilgi
    /// </summary>
    public decimal? CommissionRate { get; set; }
    
    /// <summary>
    /// Satış tarihi
    /// </summary>
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Satış notları
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Satış durumu
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    
    /// <summary>
    /// Satışı gerçekleştiren kullanıcı ID'si
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Satışı gerçekleştiren kullanıcı (Navigation Property)
    /// </summary>
    public AppUser User { get; set; } = null!;
} 