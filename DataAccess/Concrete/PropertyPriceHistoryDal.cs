using DataAccess.Abstract;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class PropertyPriceHistoryDal : GenericRepository<PropertyPriceHistory>, IPropertyPriceHistoryDal
    {
        public PropertyPriceHistoryDal(DbContext context) : base(context)
        {
        }

        public async Task<List<PropertyPriceHistory>> GetPriceHistoryByPropertyAsync(int propertyId)
        {
            return await _dbSet
                .Where(pph => pph.PropertyId == propertyId)
                .OrderByDescending(pph => pph.Date)
                .ToListAsync();
        }

        public async Task<PropertyPriceHistory?> GetLatestPriceByPropertyAsync(int propertyId)
        {
            return await _dbSet
                .Where(pph => pph.PropertyId == propertyId)
                .OrderByDescending(pph => pph.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PropertyPriceHistory>> GetPriceHistoryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(pph => pph.Date >= startDate && pph.Date <= endDate)
                .OrderByDescending(pph => pph.Date)
                .ToListAsync();
        }
    }
} 