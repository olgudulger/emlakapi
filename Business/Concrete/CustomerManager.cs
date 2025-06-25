using AutoMapper;
using Business.Abstract;
using Business.DTOs.Customer;
using DataAccess.Abstract;
using Entity.Models;
using Entity.Enums;

namespace Business.Concrete;

public class CustomerManager : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerListDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerListDto>>(customers);
    }

    public async Task<CustomerDetailDto?> GetByIdAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        return customer == null ? null : _mapper.Map<CustomerDetailDto>(customer);
    }

    public async Task<CustomerDetailDto> CreateAsync(CustomerCreateDto customerCreateDto)
    {
        var customer = _mapper.Map<Customer>(customerCreateDto);
        customer.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        // Province preferences ekle
        await AddProvincePreferencesAsync(customer.Id, customerCreateDto.ProvincePreferences);

        return await GetByIdAsync(customer.Id) ?? throw new InvalidOperationException("Müşteri oluşturulamadı");
    }

    public async Task<CustomerDetailDto?> UpdateAsync(int id, CustomerUpdateDto customerUpdateDto)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) return null;

        _mapper.Map(customerUpdateDto, customer);
        customer.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        // Mevcut province preferences'ları temizle ve yenilerini ekle
        await ClearProvincePreferencesAsync(id);
        await AddProvincePreferencesAsync(id, customerUpdateDto.ProvincePreferences);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) return false;

        // Province preferences'ları da temizle
        await ClearProvincePreferencesAsync(id);
        
        await _unitOfWork.Customers.DeleteAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CustomerListDto>> GetByInterestTypeAsync(string interestType)
    {
        // Database-side filtreleme kullan
        var interestTypeEnum = Enum.Parse<InterestType>(interestType);
        var customers = await _unitOfWork.Customers.GetCustomersByInterestTypeAsync(interestTypeEnum);
        return _mapper.Map<IEnumerable<CustomerListDto>>(customers);
    }

    public async Task<IEnumerable<CustomerListDto>> GetByCustomerTypeAsync(string customerType)
    {
        // Database-side filtreleme kullan
        var customerTypeEnum = Enum.Parse<CustomerType>(customerType);
        var customers = await _unitOfWork.Customers.GetAllAsync(c => c.CustomerType == customerTypeEnum);
        return _mapper.Map<IEnumerable<CustomerListDto>>(customers);
    }

    public async Task<IEnumerable<CustomerListDto>> SearchAsync(string searchTerm)
    {
        // Database-side filtreleme kullan
        var customers = await _unitOfWork.Customers.GetAllAsync(c => 
            c.FullName.Contains(searchTerm) || 
            c.Phone.Contains(searchTerm));
        return _mapper.Map<IEnumerable<CustomerListDto>>(customers);
    }

    public async Task<bool> AddProvincePreferenceAsync(int customerId, int provinceId)
    {
        var preferences = await _unitOfWork.CustomerProvincePreferences.GetAllAsync();
        var exists = preferences.Any(x => x.CustomerId == customerId && x.ProvinceId == provinceId);

        if (exists) return false;

        var preference = new CustomerProvincePreference
        {
            CustomerId = customerId,
            ProvinceId = provinceId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CustomerProvincePreferences.AddAsync(preference);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveProvincePreferenceAsync(int customerId, int provinceId)
    {
        var preferences = await _unitOfWork.CustomerProvincePreferences.GetAllAsync();
        var preference = preferences.FirstOrDefault(x => x.CustomerId == customerId && x.ProvinceId == provinceId);

        if (preference == null) return false;

        await _unitOfWork.CustomerProvincePreferences.DeleteAsync(preference);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task AddProvincePreferencesAsync(int customerId, IEnumerable<ProvincePreferenceDto> provincePreferences)
    {
        foreach (var provincePreferenceDto in provincePreferences)
        {
            // Province preference oluştur
            var customerProvincePreference = new CustomerProvincePreference
            {
                CustomerId = customerId,
                ProvinceId = provincePreferenceDto.ProvinceId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CustomerProvincePreferences.AddAsync(customerProvincePreference);
            await _unitOfWork.SaveChangesAsync(); // ID'yi almak için save ediyoruz

            // District preferences oluştur
            foreach (var districtId in provincePreferenceDto.DistrictIds)
            {
                var customerDistrictPreference = new CustomerDistrictPreference
                {
                    CustomerProvincePreferenceId = customerProvincePreference.Id,
                    DistrictId = districtId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.CustomerDistrictPreferences.AddAsync(customerDistrictPreference);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ClearProvincePreferencesAsync(int customerId)
    {
        var provincePreferences = await _unitOfWork.CustomerProvincePreferences.GetAllAsync();
        var customerProvincePreferences = provincePreferences.Where(x => x.CustomerId == customerId);

        foreach (var provincePreference in customerProvincePreferences)
        {
            // İlgili district preferences'ları da sil
            var districtPreferences = await _unitOfWork.CustomerDistrictPreferences.GetAllAsync();
            var customerDistrictPreferences = districtPreferences.Where(x => x.CustomerProvincePreferenceId == provincePreference.Id);

            foreach (var districtPreference in customerDistrictPreferences)
            {
                await _unitOfWork.CustomerDistrictPreferences.DeleteAsync(districtPreference);
            }

            await _unitOfWork.CustomerProvincePreferences.DeleteAsync(provincePreference);
        }

        await _unitOfWork.SaveChangesAsync();
    }
} 