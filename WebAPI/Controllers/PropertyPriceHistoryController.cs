using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Abstract;
using Business.DTOs.Property;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertyPriceHistoryController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyPriceHistoryController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// Belirtilen emlağın fiyat geçmişini getirir
    /// </summary>
    /// <param name="propertyId">Emlak ID</param>
    /// <returns>Fiyat geçmişi listesi</returns>
    [HttpGet("{propertyId}")]
    public async Task<ActionResult<IEnumerable<PriceHistoryDto>>> GetPriceHistory(int propertyId)
    {
        try
        {
            var priceHistory = await _propertyService.GetPriceHistoryAsync(propertyId);
            return Ok(priceHistory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Fiyat geçmişi alınırken hata oluştu: {ex.Message}");
        }
    }

    /// <summary>
    /// Emlağa manuel fiyat geçmişi kaydı ekler (Admin yetkisi gerekir)
    /// </summary>
    /// <param name="propertyId">Emlak ID</param>
    /// <param name="price">Yeni fiyat</param>
    /// <returns>İşlem sonucu</returns>
    [HttpPost("{propertyId}/add-price")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddPriceHistory(int propertyId, [FromBody] decimal price)
    {
        try
        {
            if (price <= 0)
            {
                return BadRequest("Fiyat 0'dan büyük olmalıdır.");
            }

            var result = await _propertyService.AddPriceHistoryAsync(propertyId, price);
            
            if (!result)
            {
                return BadRequest("Fiyat geçmişi eklenemedi. Emlak bulunamadı veya aynı fiyat zaten mevcut.");
            }

            return Ok(new { Message = "Fiyat geçmişi başarıyla eklendi.", Price = price });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Fiyat geçmişi eklenirken hata oluştu: {ex.Message}");
        }
    }

    /// <summary>
    /// Belirtilen tarih aralığındaki tüm fiyat değişikliklerini getirir (Admin yetkisi gerekir)
    /// </summary>
    /// <param name="startDate">Başlangıç tarihi</param>
    /// <param name="endDate">Bitiş tarihi</param>
    /// <returns>Tarih aralığındaki fiyat geçmişi</returns>
    [HttpGet("date-range")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PriceHistoryDto>>> GetPriceHistoryByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
            {
                return BadRequest("Başlangıç tarihi bitiş tarihinden küçük olmalıdır.");
            }

            // Bu metod için PropertyPriceHistoryDal'dan doğrudan çağıracağız
            // Çünkü PropertyService'de bu metod bulunmuyor
            return BadRequest("Bu özellik henüz implement edilmemiş.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Fiyat geçmişi alınırken hata oluştu: {ex.Message}");
        }
    }
} 