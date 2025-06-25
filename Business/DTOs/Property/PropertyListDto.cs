using Entity.Enums;

namespace Business.DTOs.Property;

public class PropertyListDto
{
    public int Id { get; set; }
    public PropertyType PropertyType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string NeighborhoodName { get; set; } = string.Empty;
    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int NeighborhoodId { get; set; }
    public string IntermediaryFullName { get; set; } = string.Empty;
    public string IntermediaryPhone { get; set; } = string.Empty;
    public PropertyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Customer info
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    
    // Type specific display info
    public string? TypeSpecificInfo { get; set; }
    
    // Type specific properties
    public Dictionary<string, object> TypeSpecificProperties { get; set; } = new();
} 