using System;
using Entity.Base;

namespace Entity.Models;

public class CustomerDistrictPreference : BaseEntity
{

    public int CustomerProvincePreferenceId { get; set; }
    public CustomerProvincePreference CustomerProvincePreference { get; set; } = null!;

    public int DistrictId { get; set; }
    public District District { get; set; } = null!;
}
