using Entity.Enums;

namespace Business.DTOs.Customer;

public class CustomerListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string Notes { get; set; } = string.Empty;
    public InterestType InterestType { get; set; }
    public CustomerType CustomerType { get; set; }
    public int ProvincePreferencesCount { get; set; }
    public DateTime CreatedAt { get; set; }
} 