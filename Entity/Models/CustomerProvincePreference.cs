using System;
using Entity.Base;

namespace Entity.Models;

public class CustomerProvincePreference : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int ProvinceId { get; set; }
    public Province? Province { get; set; }

    public ICollection<CustomerDistrictPreference> DistrictPreferences { get; set; } = new List<CustomerDistrictPreference>();
}