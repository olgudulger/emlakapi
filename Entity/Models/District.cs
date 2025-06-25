using System;
using Entity.Base;

namespace Entity.Models;

public class District : BaseEntity
{
    public required string Name { get; set; }
    public int ProvinceId { get; set; }

    // Navigation Properties
    public Province? Province { get; set; }
    public List<Neighborhood> Neighborhoods { get; set; } = new();
    public List<CustomerDistrictPreference> CustomerDistrictPreferences { get; set; } = new();
}
