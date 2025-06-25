using DataAccess.Abstract;
using Entity.Models;
using Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class CustomerDal : GenericRepository<Customer>, ICustomerDal
    {
        public CustomerDal(DbContext context) : base(context)
        {
        }

        public async Task<List<Customer>> GetCustomersWithPropertiesAsync()
        {
            return await _dbSet
                .Include(c => c.Properties)
                .Include(c => c.ProvincePreferences)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerWithPropertiesAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Properties)
                .Include(c => c.ProvincePreferences)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<List<Customer>> GetCustomersByInterestTypeAsync(InterestType interestType)
        {
            return await _dbSet
                .Where(c => c.InterestType == interestType)
                .Include(c => c.Properties)
                .ToListAsync();
        }
    }
} 