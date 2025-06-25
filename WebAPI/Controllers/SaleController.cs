using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Abstract;
using Business.DTOs.Sale;
using Entity.Enums;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    /// <summary>
    /// Tüm satışları getirir
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetAll()
    {
        try
        {
            var sales = await _saleService.GetAllAsync();
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satışlar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirtilen ID'ye sahip satışı getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDetailDto>> GetById(int id)
    {
        try
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null)
                return NotFound(new { success = false, message = "Satış bulunamadı" });

            return Ok(new { success = true, data = sale });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satış getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni satış kaydı oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SaleDetailDto>> Create([FromBody] SaleCreateDto saleCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            var sale = await _saleService.CreateAsync(saleCreateDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, 
                new { success = true, data = sale, message = "Satış başarıyla oluşturuldu" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satış oluşturulamadı: {ex.Message}" });
        }
    }

    /// <summary>
    /// Mevcut satışı günceller
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SaleDetailDto>> Update(int id, [FromBody] SaleUpdateDto saleUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var sale = await _saleService.UpdateAsync(id, saleUpdateDto);
            if (sale == null)
                return NotFound(new { success = false, message = "Satış bulunamadı" });

            return Ok(new { success = true, data = sale, message = "Satış başarıyla güncellendi" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satış güncellenemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Satışı siler
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _saleService.DeleteAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Satış bulunamadı" });

            return Ok(new { success = true, message = "Satış başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satış silinemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirtilen emlağın satışlarını getirir
    /// </summary>
    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetByProperty(int propertyId)
    {
        try
        {
            var sales = await _saleService.GetSalesByPropertyAsync(propertyId);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Emlak satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirtilen satıcı müşterinin satışlarını getirir
    /// </summary>
    [HttpGet("seller-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetBySellerCustomer(int customerId)
    {
        try
        {
            var sales = await _saleService.GetSalesBySellerCustomerAsync(customerId);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Satıcı satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirtilen alıcı müşterinin satışlarını getirir
    /// </summary>
    [HttpGet("buyer-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetByBuyerCustomer(int customerId)
    {
        try
        {
            var sales = await _saleService.GetSalesByBuyerCustomerAsync(customerId);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Alıcı satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcının satışlarını getirir
    /// </summary>
    [HttpGet("my-sales")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetMySales()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            var sales = await _saleService.GetSalesByUserAsync(userId);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Kullanıcı satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Belirtilen kullanıcının satışlarını getirir (Admin)
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetByUser(string userId)
    {
        try
        {
            var sales = await _saleService.GetSalesByUserAsync(userId);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Kullanıcı satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Tarih aralığındaki satışları getirir
    /// </summary>
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
                return BadRequest(new { success = false, message = "Başlangıç tarihi bitiş tarihinden küçük olmalıdır" });

            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Tarih aralığı satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Duruma göre satışları getirir
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetByStatus(SaleStatus status)
    {
        try
        {
            var sales = await _saleService.GetSalesByStatusAsync(status);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Durum satışları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Son satışları getirir
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<SaleListDto>>> GetRecent([FromQuery] int count = 10)
    {
        try
        {
            var sales = await _saleService.GetRecentSalesAsync(count);
            return Ok(new { success = true, data = sales });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Son satışlar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Satış istatistiklerini getirir
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<SaleStatisticsDto>> GetStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _saleService.GetSalesStatisticsAsync(startDate, endDate);
            return Ok(new { success = true, data = statistics });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"İstatistikler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcının toplam kazancını getirir
    /// </summary>
    [HttpGet("my-profit")]
    public async Task<ActionResult<decimal>> GetMyTotalProfit(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            var totalProfit = await _saleService.GetTotalProfitByUserAsync(userId, startDate, endDate);
            return Ok(new { success = true, data = totalProfit });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Toplam kazanç getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcının satış istatistiklerini getirir
    /// </summary>
    [HttpGet("my-statistics")]
    public async Task<ActionResult<SaleStatisticsDto>> GetMyStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });

            var statistics = await _saleService.GetUserSalesStatisticsAsync(userId, startDate, endDate);
            return Ok(new { success = true, data = statistics });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Kullanıcı istatistikleri getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Komisyon hesaplar
    /// </summary>
    [HttpPost("calculate-commission")]
    public async Task<ActionResult<decimal>> CalculateCommission([FromBody] CommissionCalculationDto calculationDto)
    {
        try
        {
            var commission = await _saleService.CalculateCommissionAsync(calculationDto.SalePrice, calculationDto.CommissionRate);
            return Ok(new { 
                success = true, 
                data = new {
                    salePrice = calculationDto.SalePrice,
                    commissionRate = calculationDto.CommissionRate,
                    commission = commission,
                    netProfit = commission - calculationDto.Expenses
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Komisyon hesaplanamadı: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlakın satılabilir olup olmadığını kontrol eder
    /// </summary>
    [HttpGet("can-sell/{propertyId}")]
    public async Task<ActionResult<bool>> CanSellProperty(int propertyId)
    {
        try
        {
            var canSell = await _saleService.CanSellPropertyAsync(propertyId);
            return Ok(new { success = true, data = canSell });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Kontrol edilemedi: {ex.Message}" });
        }
    }
}

/// <summary>
/// Komisyon hesaplama DTO'su
/// </summary>
public class CommissionCalculationDto
{
    public decimal SalePrice { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal Expenses { get; set; } = 0;
} 