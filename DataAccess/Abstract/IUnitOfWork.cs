using System;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Abstract
{
    public interface IUnitOfWork
    {
        // Repository interfaces
        ICustomerDal Customers { get; }
        IPropertyDal Properties { get; }
        IPropertyDocumentDal PropertyDocuments { get; }
        IPropertyPriceHistoryDal PropertyPriceHistories { get; }
        ISaleDal Sales { get; }
        IProvinceDal Provinces { get; }
        IDistrictDal Districts { get; }
        INeighborhoodDal Neighborhoods { get; }
        ICustomerProvincePreferenceDal CustomerProvincePreferences { get; }
        ICustomerDistrictPreferenceDal CustomerDistrictPreferences { get; }
        IRefreshTokenDal RefreshTokens { get; }

        DbContext Context { get; }
        Task<int> SaveChangesAsync();
    }
}
