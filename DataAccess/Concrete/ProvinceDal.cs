using DataAccess.Abstract;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class ProvinceDal : GenericRepository<Province>, IProvinceDal
    {
        public ProvinceDal(DbContext context) : base(context)
        {
        }

        public async Task<Province?> GetProvinceWithDistrictsAsync(int provinceId)
        {
            return await _dbSet
                .Include(p => p.Districts)
                .FirstOrDefaultAsync(p => p.Id == provinceId);
        }

        public async Task<List<Province>> GetAllProvincesWithDistrictsAsync()
        {
            return await _dbSet
                .Include(p => p.Districts)
                .ToListAsync();
        }

        public async Task<Province?> GetProvinceByNameAsync(string provinceName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == provinceName);
        }
    }
} 