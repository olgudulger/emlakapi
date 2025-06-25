using DataAccess.Abstract;
using DataAccess.Context;
using Entity.Models;

namespace DataAccess.Concrete;

public class CustomerProvincePreferenceDal : GenericRepository<CustomerProvincePreference>, ICustomerProvincePreferenceDal
{
    public CustomerProvincePreferenceDal(EmlakDbContext context) : base(context)
    {
    }
} 