using DataAccess.Abstract;
using DataAccess.Context;
using Entity.Models;

namespace DataAccess.Concrete;

public class CustomerDistrictPreferenceDal : GenericRepository<CustomerDistrictPreference>, ICustomerDistrictPreferenceDal
{
    public CustomerDistrictPreferenceDal(EmlakDbContext context) : base(context)
    {
    }
} 