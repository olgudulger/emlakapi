using DataAccess.Abstract;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class PropertyDocumentDal : GenericRepository<PropertyDocument>, IPropertyDocumentDal
    {
        public PropertyDocumentDal(DbContext context) : base(context)
        {
        }

        public async Task<List<PropertyDocument>> GetDocumentsByPropertyAsync(int propertyId)
        {
            return await _dbSet
                .Where(pd => pd.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<PropertyDocument?> GetDocumentByPathAsync(string documentPath)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pd => pd.FilePath == documentPath);
        }
    }
} 