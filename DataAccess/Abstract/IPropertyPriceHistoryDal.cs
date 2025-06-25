using Entity.Models;

namespace DataAccess.Abstract
{
    public interface IPropertyPriceHistoryDal : IGenericRepository<PropertyPriceHistory>
    {
        Task<List<PropertyPriceHistory>> GetPriceHistoryByPropertyAsync(int propertyId);
        Task<PropertyPriceHistory?> GetLatestPriceByPropertyAsync(int propertyId);
        Task<List<PropertyPriceHistory>> GetPriceHistoryByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 