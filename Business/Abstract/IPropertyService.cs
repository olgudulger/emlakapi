using Business.DTOs.Property;
using Entity.Enums;

namespace Business.Abstract;

public interface IPropertyService
{
    Task<IEnumerable<PropertyListDto>> GetAllAsync();
    Task<PropertyDetailDto?> GetByIdAsync(int id);
    Task<PropertyDetailDto> CreateAsync(PropertyCreateDto propertyCreateDto);
    Task<PropertyDetailDto?> UpdateAsync(int id, PropertyUpdateDto propertyUpdateDto);
    Task<bool> DeleteAsync(int id);
    
    Task<IEnumerable<PropertyListDto>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<PropertyListDto>> GetByPropertyTypeAsync(PropertyType propertyType);
    Task<IEnumerable<PropertyListDto>> GetByLocationAsync(int provinceId, int? districtId = null);
    Task<IEnumerable<PropertyListDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<PropertyListDto>> GetByStatusAsync(PropertyStatus status);
    
    // Price history
    Task<bool> AddPriceHistoryAsync(int propertyId, decimal price);
    Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int propertyId);
    
    // Documents
    Task<bool> AddDocumentAsync(int propertyId, PropertyDocumentCreateDto documentDto);
    Task<bool> RemoveDocumentAsync(int documentId);
    Task<IEnumerable<PropertyDocumentDto>> GetDocumentsAsync(int propertyId);
} 