using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Abstract;
using Entity.Models;
using Business.DTOs.Location;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NeighborhoodController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public NeighborhoodController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Tüm mahalleleri getirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? districtId = null)
    {
        try
        {
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            
            if (districtId.HasValue)
            {
                neighborhoods = neighborhoods.Where(n => n.DistrictId == districtId.Value).ToList();
            }

            return Ok(new { success = true, data = neighborhoods });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Mahalleler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// ID'ye göre mahalle detayını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var neighborhood = await _unitOfWork.Neighborhoods.GetByIdAsync(id);
            
            if (neighborhood == null)
                return NotFound(new { success = false, message = $"ID {id} ile mahalle bulunamadı" });

            return Ok(new { success = true, data = neighborhood });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Mahalle detayı getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni mahalle oluşturur
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] NeighborhoodCreateDto neighborhoodCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            // İlçe var mı kontrol et
            var district = await _unitOfWork.Districts.GetByIdAsync(neighborhoodCreateDto.DistrictId);
            if (district == null)
                return BadRequest(new { success = false, message = "Belirtilen ilçe bulunamadı" });

            // Aynı ilçe içinde mahalle adı kontrolü
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var existingNeighborhood = neighborhoods.FirstOrDefault(n => 
                n.Name == neighborhoodCreateDto.Name && n.DistrictId == neighborhoodCreateDto.DistrictId);
            if (existingNeighborhood != null)
                return BadRequest(new { success = false, message = "Bu ilçe içinde aynı isimde bir mahalle zaten mevcut" });

            var neighborhood = new Neighborhood
            {
                Name = neighborhoodCreateDto.Name,
                DistrictId = neighborhoodCreateDto.DistrictId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Neighborhoods.AddAsync(neighborhood);
            await _unitOfWork.SaveChangesAsync();

            // Circular reference'ı önlemek için navigation property'leri temizle
            neighborhood.District = null;

            return CreatedAtAction(nameof(GetById), new { id = neighborhood.Id }, 
                new { success = true, message = "Mahalle başarıyla oluşturuldu", data = neighborhood });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Mahalle oluşturulamadı: {ex.Message}" });
        }
    }

    /// <summary>
    /// Mahalle günceller
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] NeighborhoodUpdateDto neighborhoodUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var neighborhood = await _unitOfWork.Neighborhoods.GetByIdAsync(id);
            if (neighborhood == null)
                return NotFound(new { success = false, message = $"ID {id} ile mahalle bulunamadı" });

            // İlçe var mı kontrol et
            var district = await _unitOfWork.Districts.GetByIdAsync(neighborhoodUpdateDto.DistrictId);
            if (district == null)
                return BadRequest(new { success = false, message = "Belirtilen ilçe bulunamadı" });

            // Aynı ilçe içinde mahalle adı kontrolü (kendisi hariç)
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var existingNeighborhood = neighborhoods.FirstOrDefault(n => 
                n.Name == neighborhoodUpdateDto.Name && n.DistrictId == neighborhoodUpdateDto.DistrictId && n.Id != id);
            if (existingNeighborhood != null)
                return BadRequest(new { success = false, message = "Bu ilçe içinde aynı isimde başka bir mahalle zaten mevcut" });

            neighborhood.Name = neighborhoodUpdateDto.Name;
            neighborhood.DistrictId = neighborhoodUpdateDto.DistrictId;
            neighborhood.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Neighborhoods.UpdateAsync(neighborhood);
            await _unitOfWork.SaveChangesAsync();

            // Circular reference'ı önlemek için navigation property'leri temizle
            neighborhood.District = null;

            return Ok(new { success = true, message = "Mahalle başarıyla güncellendi", data = neighborhood });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Mahalle güncellenemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Mahalle siler
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var neighborhood = await _unitOfWork.Neighborhoods.GetByIdAsync(id);
            if (neighborhood == null)
                return NotFound(new { success = false, message = $"ID {id} ile mahalle bulunamadı" });

            // Mahalle'ye bağlı property kontrolü
            var properties = await _unitOfWork.Properties.GetAllAsync();
            var neighborhoodProperties = properties.Where(p => p.NeighborhoodId == id);
            if (neighborhoodProperties.Any())
                return BadRequest(new { success = false, message = "Bu mahalle'ye bağlı emlaklar mevcut. Önce emlakları silin veya başka mahalleye taşıyın." });

            await _unitOfWork.Neighborhoods.DeleteAsync(neighborhood);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = "Mahalle başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Mahalle silinemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İlçe ID'sine göre mahalleleri getirir
    /// </summary>
    [HttpGet("by-district/{districtId}")]
    public async Task<IActionResult> GetByDistrictId(int districtId)
    {
        try
        {
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var districtNeighborhoods = neighborhoods.Where(n => n.DistrictId == districtId);

            return Ok(new { success = true, data = districtNeighborhoods });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe mahalleleri getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İl ID'sine göre tüm mahalleleri getirir (il -> ilçe -> mahalle)
    /// </summary>
    [HttpGet("by-province/{provinceId}")]
    public async Task<IActionResult> GetByProvinceId(int provinceId)
    {
        try
        {
            var districts = await _unitOfWork.Districts.GetAllAsync();
            var provinceDistricts = districts.Where(d => d.ProvinceId == provinceId).Select(d => d.Id).ToList();
            
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var provinceNeighborhoods = neighborhoods.Where(n => provinceDistricts.Contains(n.DistrictId));

            return Ok(new { success = true, data = provinceNeighborhoods });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl mahalleleri getirilemedi: {ex.Message}" });
        }
    }
} 