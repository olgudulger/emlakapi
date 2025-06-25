using Entity.Models;

namespace DataAccess.Abstract
{
    public interface ICustomerDal : IGenericRepository<Customer>
    {
        Task<List<Customer>> GetCustomersWithPropertiesAsync();
        Task<Customer?> GetCustomerWithPropertiesAsync(int customerId);
        Task<List<Customer>> GetCustomersByInterestTypeAsync(Entity.Enums.InterestType interestType);
    }
} 