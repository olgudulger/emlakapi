using System;
using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class Customer : BaseEntity
{
    public required string FullName { get; set; }
    public required string Phone { get; set; }
    public required decimal Budget { get; set; }
    public string Notes { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; } = CustomerType.AlıcıSatıcı;
    public required ICollection<CustomerProvincePreference> ProvincePreferences { get; set; }
    public InterestType InterestType { get; set; } = InterestType.Tümü;
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
