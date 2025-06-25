using Entity.Enums;

namespace Business.DTOs.Customer;

public class CustomerDetailDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string Notes { get; set; } = string.Empty;
    public InterestType InterestType { get; set; }
    public CustomerType CustomerType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<ProvincePreferenceDetailDto> ProvincePreferences { get; set; } = new();
    public List<PropertySummaryDto> Properties { get; set; } = new();
}

public class ProvincePreferenceDetailDto
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public List<DistrictPreferenceDto> DistrictPreferences { get; set; } = new();
}

public class DistrictPreferenceDto
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
}

public class PropertySummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public PropertyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
} 