using Entity.Enums;

namespace Business.DTOs.Property;

public class PropertyDetailDto
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
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Customer info
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Type specific properties
    public PropertyTypeSpecificDto? TypeSpecificData { get; set; }
    
    // Related data
    public List<PriceHistoryDto> PriceHistory { get; set; } = new();
    public List<PropertyDocumentDto> Documents { get; set; } = new();
}

public class PropertyTypeSpecificDto
{
    public PropertyType PropertyType { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class PriceHistoryDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PropertyDocumentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PropertyDocumentCreateDto
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
} 