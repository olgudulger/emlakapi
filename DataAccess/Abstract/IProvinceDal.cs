using Entity.Models;

namespace DataAccess.Abstract
{
    public interface IProvinceDal : IGenericRepository<Province>
    {
        Task<Province?> GetProvinceWithDistrictsAsync(int provinceId);
        Task<List<Province>> GetAllProvincesWithDistrictsAsync();
        Task<Province?> GetProvinceByNameAsync(string provinceName);
    }
} 