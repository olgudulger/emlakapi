using Entity.Base;
using Entity.Enums;

namespace DataAccess.Abstract
{
    public interface IPropertyDal : IGenericRepository<Property>
    {
        Task<List<Property>> GetPropertiesByCustomerAsync(int customerId);
        Task<List<Property>> GetPropertiesByTypeAsync(PropertyType propertyType);
        Task<List<Property>> GetPropertiesByLocationAsync(int provinceId, int? districtId = null);
        Task<List<Property>> GetPropertiesByStatusAsync(PropertyStatus status);
        Task<Property?> GetPropertyWithDocumentsAsync(int propertyId);
        Task<Property?> GetPropertyWithPriceHistoryAsync(int propertyId);
        
        // Yeni performans optimize edilmiş method'lar
        Task<List<Property>> GetPropertiesByMultipleStatusAsync(PropertyStatus[] statuses);
        Task<List<Property>> GetPropertiesPaginatedAsync(int skip, int take, PropertyStatus? status = null);
        Task<int> GetPropertiesCountAsync(PropertyStatus? status = null);
        Task<List<Property>> SearchPropertiesAsync(string searchTerm, int skip = 0, int take = 50);
    }
} 