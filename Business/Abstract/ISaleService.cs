using Business.DTOs.Sale;
using Entity.Enums;

namespace Business.Abstract;

public interface ISaleService
{
    // CRUD Operations
    Task<IEnumerable<SaleListDto>> GetAllAsync();
    Task<SaleDetailDto?> GetByIdAsync(int id);
    Task<SaleDetailDto> CreateAsync(SaleCreateDto saleCreateDto, string userId);
    Task<SaleDetailDto?> UpdateAsync(int id, SaleUpdateDto saleUpdateDto);
    Task<bool> DeleteAsync(int id);

    // Query Operations
    Task<IEnumerable<SaleListDto>> GetSalesByPropertyAsync(int propertyId);
    Task<IEnumerable<SaleListDto>> GetSalesBySellerCustomerAsync(int sellerCustomerId);
    Task<IEnumerable<SaleListDto>> GetSalesByBuyerCustomerAsync(int buyerCustomerId);
    Task<IEnumerable<SaleListDto>> GetSalesByUserAsync(string userId);
    Task<IEnumerable<SaleListDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SaleListDto>> GetSalesByStatusAsync(SaleStatus status);
    Task<IEnumerable<SaleListDto>> GetRecentSalesAsync(int count = 10);

    // Statistics and Analytics
    Task<SaleStatisticsDto> GetSalesStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetTotalProfitByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SaleStatisticsDto> GetUserSalesStatisticsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);

    // Business Logic
    Task<bool> CanSellPropertyAsync(int propertyId);
    Task<decimal> CalculateCommissionAsync(decimal salePrice, decimal commissionRate);
    Task<bool> MarkPropertyAsSoldAsync(int propertyId);
    Task<bool> UpdatePropertyStatusBasedOnSalesAsync(int propertyId);
} 