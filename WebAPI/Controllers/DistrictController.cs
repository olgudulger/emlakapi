using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Abstract;
using Entity.Models;
using Business.DTOs.Location;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DistrictController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public DistrictController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Tüm ilçeleri getirir
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? provinceId = null)
    {
        try
        {
            var districts = await _unitOfWork.Districts.GetAllAsync();
            
            if (provinceId.HasValue)
            {
                districts = districts.Where(d => d.ProvinceId == provinceId.Value).ToList();
            }

            return Ok(new { success = true, data = districts });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçeler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// ID'ye göre ilçe detayını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            
            if (district == null)
                return NotFound(new { success = false, message = $"ID {id} ile ilçe bulunamadı" });

            return Ok(new { success = true, data = district });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe detayı getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni ilçe oluşturur
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] DistrictCreateDto districtCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            // İl var mı kontrol et
            var province = await _unitOfWork.Provinces.GetByIdAsync(districtCreateDto.ProvinceId);
            if (province == null)
                return BadRequest(new { success = false, message = "Belirtilen il bulunamadı" });

            // Aynı il içinde ilçe adı kontrolü
            var districts = await _unitOfWork.Districts.GetAllAsync();
            var existingDistrict = districts.FirstOrDefault(d => 
                d.Name == districtCreateDto.Name && d.ProvinceId == districtCreateDto.ProvinceId);
            if (existingDistrict != null)
                return BadRequest(new { success = false, message = "Bu il içinde aynı isimde bir ilçe zaten mevcut" });

            var district = new District
            {
                Name = districtCreateDto.Name,
                ProvinceId = districtCreateDto.ProvinceId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Districts.AddAsync(district);
            await _unitOfWork.SaveChangesAsync();

            // Circular reference'ı önlemek için navigation property'leri temizle
            district.Province = null;

            return CreatedAtAction(nameof(GetById), new { id = district.Id }, 
                new { success = true, message = "İlçe başarıyla oluşturuldu", data = district });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe oluşturulamadı: {ex.Message}" });
        }
    }

    /// <summary>
    /// İlçe günceller
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] DistrictUpdateDto districtUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            if (district == null)
                return NotFound(new { success = false, message = $"ID {id} ile ilçe bulunamadı" });

            // İl var mı kontrol et
            var province = await _unitOfWork.Provinces.GetByIdAsync(districtUpdateDto.ProvinceId);
            if (province == null)
                return BadRequest(new { success = false, message = "Belirtilen il bulunamadı" });

            // Aynı il içinde ilçe adı kontrolü (kendisi hariç)
            var districts = await _unitOfWork.Districts.GetAllAsync();
            var existingDistrict = districts.FirstOrDefault(d => 
                d.Name == districtUpdateDto.Name && d.ProvinceId == districtUpdateDto.ProvinceId && d.Id != id);
            if (existingDistrict != null)
                return BadRequest(new { success = false, message = "Bu il içinde aynı isimde başka bir ilçe zaten mevcut" });

            district.Name = districtUpdateDto.Name;
            district.ProvinceId = districtUpdateDto.ProvinceId;
            district.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Districts.UpdateAsync(district);
            await _unitOfWork.SaveChangesAsync();

            // Circular reference'ı önlemek için navigation property'leri temizle
            district.Province = null;

            return Ok(new { success = true, message = "İlçe başarıyla güncellendi", data = district });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe güncellenemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İlçe siler
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            if (district == null)
                return NotFound(new { success = false, message = $"ID {id} ile ilçe bulunamadı" });

            // İlçe'ye bağlı mahalle kontrolü
            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var districtNeighborhoods = neighborhoods.Where(n => n.DistrictId == id);
            if (districtNeighborhoods.Any())
                return BadRequest(new { success = false, message = "Bu ilçe'ye bağlı mahalleler mevcut. Önce mahalleleri silin." });

            await _unitOfWork.Districts.DeleteAsync(district);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = "İlçe başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe silinemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İl ID'sine göre ilçeleri getirir
    /// </summary>
    [HttpGet("by-province/{provinceId}")]
    public async Task<IActionResult> GetByProvinceId(int provinceId)
    {
        try
        {
            var districts = await _unitOfWork.Districts.GetAllAsync();
            var provinceDistricts = districts.Where(d => d.ProvinceId == provinceId);

            return Ok(new { success = true, data = provinceDistricts });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl ilçeleri getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İlçeye ait mahalleleri getirir
    /// </summary>
    [HttpGet("{id}/neighborhoods")]
    public async Task<IActionResult> GetNeighborhoods(int id)
    {
        try
        {
            var district = await _unitOfWork.Districts.GetByIdAsync(id);
            
            if (district == null)
                return NotFound(new { success = false, message = $"ID {id} ile ilçe bulunamadı" });

            var neighborhoods = await _unitOfWork.Neighborhoods.GetAllAsync();
            var districtNeighborhoods = neighborhoods.Where(n => n.DistrictId == id);

            return Ok(new { success = true, data = districtNeighborhoods });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçe mahalleleri getirilemedi: {ex.Message}" });
        }
    }
} 