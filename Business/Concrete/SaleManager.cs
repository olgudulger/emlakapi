using AutoMapper;
using Business.Abstract;
using Business.DTOs.Sale;
using DataAccess.Abstract;
using Entity.Models;
using Entity.Enums;

namespace Business.Concrete;

public class SaleManager : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SaleManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SaleListDto>> GetAllAsync()
    {
        var sales = await _unitOfWork.Sales.GetAllAsync();
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<SaleDetailDto?> GetByIdAsync(int id)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(id);
        return sale == null ? null : _mapper.Map<SaleDetailDto>(sale);
    }

    public async Task<SaleDetailDto> CreateAsync(SaleCreateDto saleCreateDto, string userId)
    {
        // Emlak satılabilir mi kontrol et
        var canSell = await CanSellPropertyAsync(saleCreateDto.PropertyId);
        if (!canSell)
        {
            throw new InvalidOperationException("Bu emlak şu anda satılamaz.");
        }

        // Property ve müşteri bilgilerini kontrol et
        var property = await _unitOfWork.Properties.GetByIdAsync(saleCreateDto.PropertyId);
        if (property == null)
        {
            throw new InvalidOperationException("Emlak bulunamadı.");
        }

        var buyerCustomer = await _unitOfWork.Customers.GetByIdAsync(saleCreateDto.BuyerCustomerId);
        if (buyerCustomer == null)
        {
            throw new InvalidOperationException("Alıcı müşteri bulunamadı.");
        }

        // Satıcı ile alıcı aynı olamaz
        if (property.CustomerId == saleCreateDto.BuyerCustomerId)
        {
            throw new InvalidOperationException("Satıcı ile alıcı aynı kişi olamaz.");
        }

        var sale = _mapper.Map<Sale>(saleCreateDto);
        sale.UserId = userId;
        sale.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Sales.AddAsync(sale);
        await _unitOfWork.SaveChangesAsync(); // Önce satışı kaydet
        
        // Sonra emlak statusunu tüm satışlara göre güncelle
        await UpdatePropertyStatusBasedOnSalesAsync(saleCreateDto.PropertyId);
        await _unitOfWork.SaveChangesAsync(); // Emlak statusu değişikliğini kaydet

        return await GetByIdAsync(sale.Id) ?? throw new InvalidOperationException("Satış oluşturulamadı");
    }

    public async Task<SaleDetailDto?> UpdateAsync(int id, SaleUpdateDto saleUpdateDto)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(id);
        if (sale == null) return null;

        // Alıcı müşteri kontrolü
        var buyerCustomer = await _unitOfWork.Customers.GetByIdAsync(saleUpdateDto.BuyerCustomerId);
        if (buyerCustomer == null)
        {
            throw new InvalidOperationException("Alıcı müşteri bulunamadı.");
        }

        // Eski satış statusunu sakla
        var oldStatus = sale.Status;
        var propertyId = sale.PropertyId;

        _mapper.Map(saleUpdateDto, sale);
        sale.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(); // Önce değişiklikleri kaydet

        // Satış statusu değişirse emlak statusunu güncelle
        if (oldStatus != sale.Status)
        {
            await UpdatePropertyStatusBasedOnSalesAsync(propertyId);
            await _unitOfWork.SaveChangesAsync(); // Emlak statusu değişikliğini kaydet
        }

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(id);
        if (sale == null) return false;

        var propertyId = sale.PropertyId; // PropertyId'yi sakla

        await _unitOfWork.Sales.DeleteAsync(sale);
        await _unitOfWork.SaveChangesAsync(); // Önce satışı sil
        
        // Sonra emlak statusunu kalan satışlara göre güncelle
        await UpdatePropertyStatusBasedOnSalesAsync(propertyId);
        await _unitOfWork.SaveChangesAsync(); // Emlak statusu değişikliğini kaydet
        
        return true;
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesByPropertyAsync(int propertyId)
    {
        var sales = await _unitOfWork.Sales.GetSalesByPropertyAsync(propertyId);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesBySellerCustomerAsync(int sellerCustomerId)
    {
        var sales = await _unitOfWork.Sales.GetSalesBySellerCustomerAsync(sellerCustomerId);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesByBuyerCustomerAsync(int buyerCustomerId)
    {
        var sales = await _unitOfWork.Sales.GetSalesByBuyerCustomerAsync(buyerCustomerId);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesByUserAsync(string userId)
    {
        var sales = await _unitOfWork.Sales.GetSalesByUserAsync(userId);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetSalesByStatusAsync(SaleStatus status)
    {
        var sales = await _unitOfWork.Sales.GetSalesByStatusAsync(status);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<IEnumerable<SaleListDto>> GetRecentSalesAsync(int count = 10)
    {
        var sales = await _unitOfWork.Sales.GetRecentSalesAsync(count);
        return _mapper.Map<IEnumerable<SaleListDto>>(sales);
    }

    public async Task<SaleStatisticsDto> GetSalesStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var statistics = await _unitOfWork.Sales.GetSalesStatisticsAsync(startDate, endDate);
        return _mapper.Map<SaleStatisticsDto>(statistics);
    }

    public async Task<decimal> GetTotalProfitByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _unitOfWork.Sales.GetTotalProfitByUserAsync(userId, startDate, endDate);
    }

    public async Task<SaleStatisticsDto> GetUserSalesStatisticsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var userSales = await _unitOfWork.Sales.GetSalesByUserAsync(userId);
        
        if (startDate.HasValue)
            userSales = userSales.Where(s => s.SaleDate >= startDate.Value).ToList();
        
        if (endDate.HasValue)
            userSales = userSales.Where(s => s.SaleDate <= endDate.Value).ToList();

        if (!userSales.Any())
        {
            return new SaleStatisticsDto();
        }

        return new SaleStatisticsDto
        {
            TotalSalesCount = userSales.Count,
            TotalSalesAmount = userSales.Sum(s => s.SalePrice),
            TotalCommission = userSales.Sum(s => s.Commission),
            TotalExpenses = userSales.Sum(s => s.Expenses),
            TotalNetProfit = userSales.Sum(s => s.NetProfit),
            AverageSalePrice = userSales.Average(s => s.SalePrice),
            AverageCommission = userSales.Average(s => s.Commission),
            AverageNetProfit = userSales.Average(s => s.NetProfit)
        };
    }

    public async Task<bool> CanSellPropertyAsync(int propertyId)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
        if (property == null) return false;

        // Emlak mevcut ve satışa uygun durumda mı?
        return property.Status == PropertyStatus.Satilik ||      // Satılık
               property.Status == PropertyStatus.SatilikKiralik || // Satılık & Kiralık
               property.Status == PropertyStatus.Kiralik ||       // Kiralık (kiralama için)
               property.Status == PropertyStatus.Rezerv;           // Rezerv
    }

    public async Task<decimal> CalculateCommissionAsync(decimal salePrice, decimal commissionRate)
    {
        await Task.CompletedTask; // Async method için
        return salePrice * (commissionRate / 100);
    }

    public async Task<bool> MarkPropertyAsSoldAsync(int propertyId)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
        if (property == null) return false;

        property.Status = PropertyStatus.Satildi;
        property.UpdatedAt = DateTime.UtcNow;
        
        return true;
    }

    public async Task<bool> UpdatePropertyStatusBasedOnSalesAsync(int propertyId)
    {
        var sales = await _unitOfWork.Sales.GetSalesByPropertyAsync(propertyId);
        if (sales == null || !sales.Any()) return false;

        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
        if (property == null) return false;

        var completedSales = sales.Where(s => s.Status == SaleStatus.Completed).ToList();
        var incompletedSales = sales.Where(s => s.Status != SaleStatus.Completed).ToList();

        if (completedSales.Count > 0 && incompletedSales.Count == 0)
        {
            // Emlak durumuna göre tamamlanan satış sonrası status belirleme
            if (property.Status == PropertyStatus.Kiralik || property.Status == PropertyStatus.SatilikKiralik)
            {
                // Kiralık veya Satılık+Kiralık durumundaki emlaklar -> Kiralandı
                property.Status = PropertyStatus.Kiralandi;
            }
            else
            {
                // Satılık durumundaki emlaklar -> Satıldı
                property.Status = PropertyStatus.Satildi;
            }
            property.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        else if (completedSales.Count == 0 && incompletedSales.Count > 0)
        {
            // Completed satış yoksa, emlak orijinal durumuna geri döner
            // Kiralandi -> Kiralik, Satildi -> Satilik
            if (property.Status == PropertyStatus.Kiralandi)
            {
                property.Status = PropertyStatus.Kiralik;
            }
            else if (property.Status == PropertyStatus.Satildi)
            {
                property.Status = PropertyStatus.Satilik;
            }
            property.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        else
        {
            // Karışık durum - varsayılan duruma döner
            if (property.Status == PropertyStatus.Kiralandi)
            {
                property.Status = PropertyStatus.Kiralik;
            }
            else if (property.Status == PropertyStatus.Satildi)
            {
                property.Status = PropertyStatus.Satilik;
            }
            property.UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
} 