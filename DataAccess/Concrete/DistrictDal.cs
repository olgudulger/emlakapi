using DataAccess.Abstract;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class DistrictDal : GenericRepository<District>, IDistrictDal
    {
        public DistrictDal(DbContext context) : base(context)
        {
        }

        public async Task<List<District>> GetDistrictsByProvinceAsync(int provinceId)
        {
            return await _dbSet
                .Where(d => d.ProvinceId == provinceId)
                .Include(d => d.Province)
                .ToListAsync();
        }

        public async Task<District?> GetDistrictByNameAsync(string districtName, int provinceId)
        {
            return await _dbSet
                .Include(d => d.Province)
                .FirstOrDefaultAsync(d => d.Name == districtName && d.ProvinceId == provinceId);
        }
    }
} 