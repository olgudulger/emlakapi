using Entity.Models;
using Entity.Enums;

namespace DataAccess.Abstract;

public interface ISaleDal : IGenericRepository<Sale>
{
    /// <summary>
    /// Belirtilen emlak için satışları getirir
    /// </summary>
    Task<List<Sale>> GetSalesByPropertyAsync(int propertyId);
    
    /// <summary>
    /// Belirtilen müşteri için satışları getirir (satıcı olarak)
    /// </summary>
    Task<List<Sale>> GetSalesBySellerCustomerAsync(int sellerCustomerId);
    
    /// <summary>
    /// Belirtilen müşteri için satışları getirir (alıcı olarak)
    /// </summary>
    Task<List<Sale>> GetSalesByBuyerCustomerAsync(int buyerCustomerId);
    
    /// <summary>
    /// Belirtilen kullanıcının satışlarını getirir
    /// </summary>
    Task<List<Sale>> GetSalesByUserAsync(string userId);
    
    /// <summary>
    /// Tarih aralığına göre satışları getirir
    /// </summary>
    Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Duruma göre satışları getirir
    /// </summary>
    Task<List<Sale>> GetSalesByStatusAsync(SaleStatus status);
    
    /// <summary>
    /// Satış istatistiklerini getirir
    /// </summary>
    Task<SaleStatistics> GetSalesStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    /// <summary>
    /// En son satışları getirir
    /// </summary>
    Task<List<Sale>> GetRecentSalesAsync(int count = 10);
    
    /// <summary>
    /// Kullanıcının toplam kazancını getirir
    /// </summary>
    Task<decimal> GetTotalProfitByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    
    // Yeni performans optimize edilmiş method'lar
    Task<List<Sale>> GetSalesPaginatedAsync(int skip, int take, SaleStatus? status = null);
    Task<int> GetSalesCountAsync(SaleStatus? status = null);
    Task<List<Sale>> GetSalesByMultipleStatusAsync(SaleStatus[] statuses);
    Task<object> GetSalesSummaryAsync(string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
}

/// <summary>
/// Satış istatistikleri
/// </summary>
public class SaleStatistics
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