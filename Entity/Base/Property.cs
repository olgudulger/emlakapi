using System;
using Entity.Enums;
using Entity.Models;

namespace Entity.Base;

public abstract class Property: BaseEntity
{
    // TPH için discriminator field
    public PropertyType PropertyType { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int NeighborhoodId { get; set; }

    public string IntermediaryFullName { get; set; } = string.Empty;
    public string IntermediaryPhone { get; set; } = string.Empty;
    public PropertyStatus Status { get; set; } = PropertyStatus.Satilik;

    public string? Notes { get; set; }
    public int CustomerId { get; set; }
    
    // Navigation Properties
    public Customer? Customer { get; set; }
    public Province? Province { get; set; }
    public District? District { get; set; }
    public Neighborhood? Neighborhood { get; set; }
}
