using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Abstract;
using Entity.Models;
using Business.DTOs.Location;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvinceController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProvinceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Tüm illeri getirir
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var provinces = await _unitOfWork.Provinces.GetAllAsync();
            return Ok(new { success = true, data = provinces });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İller getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// ID'ye göre il detayını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            
            if (province == null)
                return NotFound(new { success = false, message = $"ID {id} ile il bulunamadı" });

            return Ok(new { success = true, data = province });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl detayı getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni il oluşturur
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ProvinceCreateDto provinceCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            // İl adı kontrolü
            var existingProvince = await _unitOfWork.Provinces.GetProvinceByNameAsync(provinceCreateDto.Name);
            if (existingProvince != null)
                return BadRequest(new { success = false, message = "Bu isimde bir il zaten mevcut" });

            var province = new Province
            {
                Name = provinceCreateDto.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Provinces.AddAsync(province);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = province.Id }, 
                new { success = true, message = "İl başarıyla oluşturuldu", data = province });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl oluşturulamadı: {ex.Message}" });
        }
    }

    /// <summary>
    /// İl günceller
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ProvinceUpdateDto provinceUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            if (province == null)
                return NotFound(new { success = false, message = $"ID {id} ile il bulunamadı" });

            // İl adı kontrolü (kendisi hariç)
            var existingProvince = await _unitOfWork.Provinces.GetProvinceByNameAsync(provinceUpdateDto.Name);
            if (existingProvince != null && existingProvince.Id != id)
                return BadRequest(new { success = false, message = "Bu isimde başka bir il zaten mevcut" });

            province.Name = provinceUpdateDto.Name;
            province.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Provinces.UpdateAsync(province);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = "İl başarıyla güncellendi", data = province });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl güncellenemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İl siler
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            if (province == null)
                return NotFound(new { success = false, message = $"ID {id} ile il bulunamadı" });

            // İl'e bağlı ilçe kontrolü
            var districts = await _unitOfWork.Districts.GetAllAsync();
            var provinceDistricts = districts.Where(d => d.ProvinceId == id);
            if (provinceDistricts.Any())
                return BadRequest(new { success = false, message = "Bu il'e bağlı ilçeler mevcut. Önce ilçeleri silin." });

            await _unitOfWork.Provinces.DeleteAsync(province);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = "İl başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İl silinemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İle ait ilçeleri getirir
    /// </summary>
    [HttpGet("{id}/districts")]
    public async Task<IActionResult> GetDistricts(int id)
    {
        try
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            
            if (province == null)
                return NotFound(new { success = false, message = $"ID {id} ile il bulunamadı" });

            var districts = await _unitOfWork.Districts.GetAllAsync();
            var provinceDistricts = districts.Where(d => d.ProvinceId == id);

            return Ok(new { success = true, data = provinceDistricts });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"İlçeler getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// İle ait tüm mahalleleri getirir (il -> ilçe -> mahalle)
    /// </summary>
    [HttpGet("{id}/neighborhoods")]
    public async Task<IActionResult> GetNeighborhoods(int id)
    {
        try
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            
            if (province == null)
                return NotFound(new { success = false, message = $"ID {id} ile il bulunamadı" });

            var districts = await _unitOfWork.Districts.GetAllAsync();
            var provinceDistricts = districts.Where(d => d.ProvinceId == id).Select(d => d.Id).ToList();
            
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