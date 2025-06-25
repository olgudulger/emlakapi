using Entity.Models;
using Business.DTOs.Customer;

namespace Business.Abstract;

public interface ICustomerService
{
    Task<IEnumerable<CustomerListDto>> GetAllAsync();
    Task<CustomerDetailDto?> GetByIdAsync(int id);
    Task<CustomerDetailDto> CreateAsync(CustomerCreateDto customerCreateDto);
    Task<CustomerDetailDto?> UpdateAsync(int id, CustomerUpdateDto customerUpdateDto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CustomerListDto>> GetByInterestTypeAsync(string interestType);
    Task<IEnumerable<CustomerListDto>> GetByCustomerTypeAsync(string customerType);
    Task<IEnumerable<CustomerListDto>> SearchAsync(string searchTerm);
    Task<bool> AddProvincePreferenceAsync(int customerId, int provinceId);
    Task<bool> RemoveProvincePreferenceAsync(int customerId, int provinceId);
} 