using Entity.Base;

namespace Entity.Models;

public class Neighborhood : BaseEntity
{
    public required string Name { get; set; }
    public int DistrictId { get; set; }
    
    // Navigation Properties
    public District? District { get; set; }
    public List<Property> Properties { get; set; } = new();
} 