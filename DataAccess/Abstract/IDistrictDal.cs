using Entity.Models;

namespace DataAccess.Abstract
{
    public interface IDistrictDal : IGenericRepository<District>
    {
        Task<List<District>> GetDistrictsByProvinceAsync(int provinceId);
        Task<District?> GetDistrictByNameAsync(string districtName, int provinceId);
    }
} 