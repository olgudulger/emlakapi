using DataAccess.Abstract;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly EmlakDbContext _context;
        private bool _disposed = false;

        // Repository instances
        private ICustomerDal? _customers;
        private IPropertyDal? _properties;
        private IPropertyDocumentDal? _propertyDocuments;
        private IPropertyPriceHistoryDal? _propertyPriceHistories;
        private ISaleDal? _sales;
        private IProvinceDal? _provinces;
        private IDistrictDal? _districts;
        private INeighborhoodDal? _neighborhoods;
        private IRefreshTokenDal? _refreshTokens;
        private ICustomerProvincePreferenceDal? _customerProvincePreferences;
        private ICustomerDistrictPreferenceDal? _customerDistrictPreferences;

        public UnitOfWork(EmlakDbContext context)
        {
            _context = context;
            _customers = new CustomerDal(_context);
            _properties = new PropertyDal(_context);
            _propertyDocuments = new PropertyDocumentDal(_context);
            _propertyPriceHistories = new PropertyPriceHistoryDal(_context);
            _sales = new SaleDal(_context);
            _provinces = new ProvinceDal(_context);
            _districts = new DistrictDal(_context);
            _neighborhoods = new NeighborhoodDal(_context);
            _refreshTokens = new RefreshTokenDal(_context);
            _customerProvincePreferences = new CustomerProvincePreferenceDal(_context);
            _customerDistrictPreferences = new CustomerDistrictPreferenceDal(_context);
        }

        public ICustomerDal Customers => _customers ??= new CustomerDal(_context);
        public IPropertyDal Properties => _properties ??= new PropertyDal(_context);
        public IPropertyDocumentDal PropertyDocuments => _propertyDocuments ??= new PropertyDocumentDal(_context);
        public IPropertyPriceHistoryDal PropertyPriceHistories => _propertyPriceHistories ??= new PropertyPriceHistoryDal(_context);
        public ISaleDal Sales => _sales ??= new SaleDal(_context);
        public IProvinceDal Provinces => _provinces ??= new ProvinceDal(_context);
        public IDistrictDal Districts => _districts ??= new DistrictDal(_context);
        public INeighborhoodDal Neighborhoods => _neighborhoods ??= new NeighborhoodDal(_context);
        public ICustomerProvincePreferenceDal CustomerProvincePreferences => _customerProvincePreferences ??= new CustomerProvincePreferenceDal(_context);
        public ICustomerDistrictPreferenceDal CustomerDistrictPreferences => _customerDistrictPreferences ??= new CustomerDistrictPreferenceDal(_context);
        public IRefreshTokenDal RefreshTokens => _refreshTokens ??= new RefreshTokenDal(_context);

        public DbContext Context => _context;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 