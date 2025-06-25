using DataAccess.Abstract;
using Entity.Base;
using Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class PropertyDal : GenericRepository<Property>, IPropertyDal
    {
        public PropertyDal(DbContext context) : base(context)
        {
        }

        // Base query için tekrar kullanılabilir method
        private IQueryable<Property> BaseQueryWithIncludes()
        {
            return _dbSet
                .Include(p => p.Customer)
                .Include(p => p.Province)
                .Include(p => p.District)
                .Include(p => p.Neighborhood);
        }

        // Lightweight query - sadece gerekli alanları içeren
        private IQueryable<Property> LightweightQuery()
        {
            return _dbSet
                .Include(p => p.Customer)
                .Include(p => p.Province)
                .Include(p => p.District);
        }

        public async Task<List<Property>> GetPropertiesByCustomerAsync(int customerId)
        {
            return await BaseQueryWithIncludes()
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesByTypeAsync(PropertyType propertyType)
        {
            return await BaseQueryWithIncludes()
                .Where(p => p.PropertyType == propertyType)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesByLocationAsync(int provinceId, int? districtId = null)
        {
            var query = BaseQueryWithIncludes()
                .Where(p => p.ProvinceId == provinceId);
            
            if (districtId.HasValue)
            {
                query = query.Where(p => p.DistrictId == districtId.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesByStatusAsync(PropertyStatus status)
        {
            return await BaseQueryWithIncludes()
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Optimized: Sadece gerekli navigation properties
        public async Task<Property?> GetPropertyWithDocumentsAsync(int propertyId)
        {
            return await BaseQueryWithIncludes()
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        public async Task<Property?> GetPropertyWithPriceHistoryAsync(int propertyId)
        {
            return await BaseQueryWithIncludes()
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        // Yeni performans optimize edilmiş method'lar
        public async Task<List<Property>> GetPropertiesByMultipleStatusAsync(PropertyStatus[] statuses)
        {
            return await BaseQueryWithIncludes()
                .Where(p => statuses.Contains(p.Status))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesPaginatedAsync(int skip, int take, PropertyStatus? status = null)
        {
            var query = LightweightQuery();
            
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetPropertiesCountAsync(PropertyStatus? status = null)
        {
            var query = _dbSet.AsQueryable();
            
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            return await query.CountAsync();
        }

        // Search için optimize edilmiş method
        public async Task<List<Property>> SearchPropertiesAsync(string searchTerm, int skip = 0, int take = 50)
        {
            return await LightweightQuery()
                .Where(p => p.Title.Contains(searchTerm) || 
                           p.Customer.FullName.Contains(searchTerm) ||
                           p.Province.Name.Contains(searchTerm) ||
                           p.District.Name.Contains(searchTerm))
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
} 