using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Abstract;
using Business.DTOs.Property;
using Entity.Enums;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// Tüm emlakları getirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var properties = await _propertyService.GetAllAsync();
            return Ok(new { success = true, data = properties });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlaklar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// ID'ye göre emlak detayını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var property = await _propertyService.GetByIdAsync(id);
            
            if (property == null)
                return NotFound(new { success = false, message = $"ID {id} ile emlak bulunamadı" });

            return Ok(new { success = true, data = property });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlak detayı getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni emlak oluşturur
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertyCreateDto propertyCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var property = await _propertyService.CreateAsync(propertyCreateDto);
            return CreatedAtAction(nameof(GetById), new { id = property.Id }, 
                new { success = true, data = property, message = "Emlak başarıyla oluşturuldu" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlak oluşturulurken hata: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak bilgilerini günceller
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PropertyUpdateDto propertyUpdateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var property = await _propertyService.UpdateAsync(id, propertyUpdateDto);
            
            if (property == null)
                return NotFound(new { success = false, message = $"ID {id} ile emlak bulunamadı" });

            return Ok(new { success = true, data = property, message = "Emlak başarıyla güncellendi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlak güncellenirken hata: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlağı siler
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _propertyService.DeleteAsync(id);
            
            if (!result)
                return NotFound(new { success = false, message = $"ID {id} ile emlak bulunamadı" });

            return Ok(new { success = true, message = "Emlak başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlak silinirken hata: {ex.Message}" });
        }
    }

    /// <summary>
    /// Müşteriye ait emlakları getirir
    /// </summary>
    [HttpGet("by-customer/{customerId}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        try
        {
            var properties = await _propertyService.GetByCustomerIdAsync(customerId);
            return Ok(new { success = true, data = properties });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Müşteri emlakları getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak türüne göre emlakları getirir
    /// </summary>
    [HttpGet("by-type/{propertyType}")]
    public async Task<IActionResult> GetByPropertyType(PropertyType propertyType)
    {
        try
        {
            var properties = await _propertyService.GetByPropertyTypeAsync(propertyType);
            return Ok(new { success = true, data = properties });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Emlak türü filtrelemesi başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Lokasyona göre emlakları getirir
    /// </summary>
    [HttpGet("by-location")]
    public async Task<IActionResult> GetByLocation([FromQuery] int provinceId, [FromQuery] int? districtId = null)
    {
        var properties = await _propertyService.GetByLocationAsync(provinceId, districtId);
        return Ok(properties);
    }

    /// <summary>
    /// Emlak durumuna göre emlakları getirir
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetByStatus(PropertyStatus status)
    {
        try
        {
            var properties = await _propertyService.GetByStatusAsync(status);
            return Ok(new { success = true, data = properties });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Durum filtrelemesi başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak araması yapar
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return BadRequest(new { success = false, message = "Arama terimi gereklidir" });

        try
        {
            var properties = await _propertyService.SearchAsync(searchTerm);
            return Ok(new { success = true, data = properties });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Arama başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak fiyat geçmişini getirir
    /// </summary>
    [HttpGet("{id}/price-history")]
    public async Task<IActionResult> GetPriceHistory(int id)
    {
        try
        {
            var priceHistory = await _propertyService.GetPriceHistoryAsync(id);
            return Ok(new { success = true, data = priceHistory });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fiyat geçmişi getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak fiyat geçmişine yeni kayıt ekler
    /// </summary>
    [HttpPost("{id}/price-history")]
    public async Task<IActionResult> AddPriceHistory(int id, [FromBody] decimal price)
    {
        if (price <= 0)
            return BadRequest(new { success = false, message = "Geçerli bir fiyat giriniz" });

        try
        {
            var result = await _propertyService.AddPriceHistoryAsync(id, price);
            
            if (!result)
                return NotFound(new { success = false, message = "Emlak bulunamadı" });

            return Ok(new { success = true, message = "Fiyat geçmişi başarıyla eklendi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fiyat geçmişi eklenirken hata: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak belgelerini getirir
    /// </summary>
    [HttpGet("{id}/documents")]
    public async Task<IActionResult> GetDocuments(int id)
    {
        try
        {
            var documents = await _propertyService.GetDocumentsAsync(id);
            return Ok(new { success = true, data = documents });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Belgeler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak belgesi ekler
    /// </summary>
    [HttpPost("{id}/documents")]
    public async Task<IActionResult> AddDocument(int id, [FromBody] PropertyDocumentCreateDto documentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var result = await _propertyService.AddDocumentAsync(id, documentDto);
            
            if (!result)
                return NotFound(new { success = false, message = "Emlak bulunamadı" });

            return Ok(new { success = true, message = "Belge başarıyla eklendi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Belge eklenirken hata: {ex.Message}" });
        }
    }

    /// <summary>
    /// Emlak belgesini siler
    /// </summary>
    [HttpDelete("documents/{documentId}")]
    public async Task<IActionResult> RemoveDocument(int documentId)
    {
        try
        {
            var result = await _propertyService.RemoveDocumentAsync(documentId);
            
            if (!result)
                return NotFound(new { success = false, message = "Belge bulunamadı" });

            return Ok(new { success = true, message = "Belge başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Belge silinirken hata: {ex.Message}" });
        }
    }
} 