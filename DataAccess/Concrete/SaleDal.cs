using DataAccess.Abstract;
using Entity.Models;
using Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete;

public class SaleDal : GenericRepository<Sale>, ISaleDal
{
    public SaleDal(DbContext context) : base(context)
    {
    }

    // Base query için tekrar kullanılabilir method - full navigation properties
    private IQueryable<Sale> BaseQueryWithIncludes()
    {
        return _dbSet
            .Include(s => s.Property)
                .ThenInclude(p => p.Customer)
            .Include(s => s.Property)
                .ThenInclude(p => p.Province)
            .Include(s => s.Property)
                .ThenInclude(p => p.District)
            .Include(s => s.Property)
                .ThenInclude(p => p.Neighborhood)
            .Include(s => s.BuyerCustomer)
            .Include(s => s.User);
    }

    // Lightweight query - sadece temel bilgiler
    private IQueryable<Sale> LightweightQuery()
    {
        return _dbSet
            .Include(s => s.Property)
                .ThenInclude(p => p.Customer)
            .Include(s => s.BuyerCustomer)
            .Include(s => s.User);
    }

    public override async Task<List<Sale>> GetAllAsync(System.Linq.Expressions.Expression<Func<Sale, bool>>? filter = null)
    {
        var query = BaseQueryWithIncludes();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.OrderByDescending(s => s.SaleDate).ToListAsync();
    }

    public override async Task<Sale?> GetByIdAsync(int id)
    {
        return await BaseQueryWithIncludes()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Sale>> GetSalesByPropertyAsync(int propertyId)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.PropertyId == propertyId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesBySellerCustomerAsync(int sellerCustomerId)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.Property.CustomerId == sellerCustomerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByBuyerCustomerAsync(int buyerCustomerId)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.BuyerCustomerId == buyerCustomerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByUserAsync(string userId)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByStatusAsync(SaleStatus status)
    {
        return await BaseQueryWithIncludes()
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    // Optimize edilmiş istatistik method - sadece gerekli alanları çekiyor
    public async Task<SaleStatistics> GetSalesStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);

        // Projection kullanarak sadece gerekli alanları çek
        var salesData = await query
            .Select(s => new 
            {
                s.SalePrice,
                s.Commission,
                s.Expenses,
                s.NetProfit
            })
            .ToListAsync();

        if (!salesData.Any())
        {
            return new SaleStatistics();
        }

        return new SaleStatistics
        {
            TotalSalesCount = salesData.Count,
            TotalSalesAmount = salesData.Sum(s => s.SalePrice),
            TotalCommission = salesData.Sum(s => s.Commission),
            TotalExpenses = salesData.Sum(s => s.Expenses),
            TotalNetProfit = salesData.Sum(s => s.NetProfit),
            AverageSalePrice = salesData.Average(s => s.SalePrice),
            AverageCommission = salesData.Average(s => s.Commission),
            AverageNetProfit = salesData.Average(s => s.NetProfit)
        };
    }

    public async Task<List<Sale>> GetRecentSalesAsync(int count = 10)
    {
        return await LightweightQuery()
            .OrderByDescending(s => s.SaleDate)
            .Take(count)
            .ToListAsync();
    }

    // Optimize edilmiş kar hesaplama - sadece gerekli alanları çekiyor
    public async Task<decimal> GetTotalProfitByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet
            .Where(s => s.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);

        return await query.SumAsync(s => s.NetProfit);
    }

    // Yeni performans optimize edilmiş method'lar
    public async Task<List<Sale>> GetSalesPaginatedAsync(int skip, int take, SaleStatus? status = null)
    {
        var query = LightweightQuery();
        
        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        return await query
            .OrderByDescending(s => s.SaleDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetSalesCountAsync(SaleStatus? status = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<List<Sale>> GetSalesByMultipleStatusAsync(SaleStatus[] statuses)
    {
        return await BaseQueryWithIncludes()
            .Where(s => statuses.Contains(s.Status))
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    // Dashboard için optimize edilmiş özet bilgiler
    public async Task<object> GetSalesSummaryAsync(string? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(s => s.UserId == userId);
            
        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);

        return await query
            .GroupBy(x => 1)
            .Select(g => new
            {
                TotalSales = g.Count(),
                CompletedSales = g.Count(s => s.Status == SaleStatus.Completed),
                PendingSales = g.Count(s => s.Status == SaleStatus.Pending),
                CancelledSales = g.Count(s => s.Status == SaleStatus.Cancelled),
                TotalRevenue = g.Sum(s => s.SalePrice),
                TotalProfit = g.Sum(s => s.NetProfit),
                TotalCommission = g.Sum(s => s.Commission)
            })
            .FirstOrDefaultAsync() ?? new
            {
                TotalSales = 0,
                CompletedSales = 0,
                PendingSales = 0,
                CancelledSales = 0,
                TotalRevenue = 0m,
                TotalProfit = 0m,
                TotalCommission = 0m
            };
    }
} 