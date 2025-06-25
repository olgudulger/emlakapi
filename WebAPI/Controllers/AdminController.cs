using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Entity.Models;
using Business.DTOs.Auth;
using Business.DTOs.User;
using DataAccess.Context;
using DataAccess.Concrete;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly EmlakDbContext _context;

    public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, EmlakDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    /// <summary>
    /// Tüm kullanıcıları getirir
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = _userManager.Users.ToList();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    Roles = roles,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd
                });
            }

            return Ok(new { success = true, data = userList });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcılar getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcı detayını getirir
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                success = true,
                data = new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    Roles = roles,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı detayı getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni kullanıcı oluşturur
    /// </summary>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            // Username kontrol et
            var existingUserByUsername = await _userManager.FindByNameAsync(userCreateDto.Username);
            if (existingUserByUsername != null)
                return BadRequest(new { success = false, message = "Bu kullanıcı adı zaten kullanılıyor" });

            // Email kontrol et
            var existingUserByEmail = await _userManager.FindByEmailAsync(userCreateDto.Email);
            if (existingUserByEmail != null)
                return BadRequest(new { success = false, message = "Bu e-posta adresi zaten kullanılıyor" });

            // Rol var mı kontrol et
            var roleExists = await _roleManager.RoleExistsAsync(userCreateDto.Role);
            if (!roleExists)
                return BadRequest(new { success = false, message = "Belirtilen rol mevcut değil" });

            // Kullanıcı oluştur
            var user = new AppUser
            {
                UserName = userCreateDto.Username,
                Email = userCreateDto.Email,
                Role = userCreateDto.Role,
                EmailConfirmed = true // Admin tarafından oluşturulduğu için direkt onaylı
            };

            var result = await _userManager.CreateAsync(user, userCreateDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { success = false, message = "Kullanıcı oluşturulamadı", errors = errors });
            }

            // Kullanıcıya rol ver
            await _userManager.AddToRoleAsync(user, userCreateDto.Role);

            return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, 
                new { 
                    success = true, 
                    message = "Kullanıcı başarıyla oluşturuldu", 
                    data = new { 
                        Id = user.Id, 
                        Username = user.UserName, 
                        Email = user.Email, 
                        Role = user.Role 
                    }
                });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı oluşturma başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcı rolünü değiştirir
    /// </summary>
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] string newRole)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Mevcut rolleri kaldır
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            
            // Yeni rolü ekle
            await _userManager.AddToRoleAsync(user, newRole);
            
            // User entity'sindeki Role field'ını da güncelle
            user.Role = newRole;
            await _userManager.UpdateAsync(user);

            return Ok(new { success = true, message = "Kullanıcı rolü başarıyla değiştirildi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Rol değiştirme başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller (username, email, role)
    /// </summary>
    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserUpdateDto userUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            // Username kontrol et (kendisi hariç)
            var existingUserByUsername = await _userManager.FindByNameAsync(userUpdateDto.Username);
            if (existingUserByUsername != null && existingUserByUsername.Id != userId)
                return BadRequest(new { success = false, message = "Bu kullanıcı adı zaten kullanılıyor" });

            // Email kontrol et (kendisi hariç)
            var existingUserByEmail = await _userManager.FindByEmailAsync(userUpdateDto.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != userId)
                return BadRequest(new { success = false, message = "Bu e-posta adresi zaten kullanılıyor" });

            // Kullanıcı bilgilerini güncelle
            user.UserName = userUpdateDto.Username;
            user.Email = userUpdateDto.Email;
            
            // Role güncelleme (eğer belirtilmişse)
            if (!string.IsNullOrEmpty(userUpdateDto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, userUpdateDto.Role);
                user.Role = userUpdateDto.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { success = false, message = "Kullanıcı güncellenemedi", errors = result.Errors });

            return Ok(new { success = true, message = "Kullanıcı başarıyla güncellendi", data = new { 
                Id = user.Id, 
                Username = user.UserName, 
                Email = user.Email, 
                Role = user.Role 
            }});
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı güncelleme başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcıyı aktif/pasif yapar
    /// </summary>
    [HttpPut("users/{userId}/lock")]
    public async Task<IActionResult> LockUnlockUser(string userId, [FromBody] bool lockUser)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            if (lockUser)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            var action = lockUser ? "kilitlendi" : "kilidi açıldı";
            return Ok(new { success = true, message = $"Kullanıcı başarıyla {action}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı kilit işlemi başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kullanıcıyı siler
    /// </summary>
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            // Admin kullanıcısı silinemez
            if (user.Role == "Admin")
                return BadRequest(new { success = false, message = "Admin kullanıcısı silinemez" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { success = false, message = "Kullanıcı silinemedi", errors = result.Errors });

            return Ok(new { success = true, message = "Kullanıcı başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Kullanıcı silme işlemi başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Admin tarafından kullanıcı şifresini değiştirir
    /// </summary>
    [HttpPut("users/{userId}/password")]
    public async Task<IActionResult> ChangeUserPassword(string userId, [FromBody] AdminPasswordChangeDto passwordChangeDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz veri", errors = ModelState });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            // Admin tarafından şifre değiştirme (eski şifre gerekmez)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passwordChangeDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { success = false, message = "Şifre değiştirilemedi", errors = errors });
            }

            return Ok(new { success = true, message = "Kullanıcı şifresi başarıyla değiştirildi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Şifre değiştirme başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// Tüm rolleri getirir
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(new { success = true, data = roles });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Roller getirilemedi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Yeni rol oluşturur
    /// </summary>
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        try
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest(new { success = false, message = "Rol adı gereklidir" });

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return BadRequest(new { success = false, message = "Bu rol zaten mevcut" });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return BadRequest(new { success = false, message = "Rol oluşturulamadı", errors = result.Errors });

            return Ok(new { success = true, message = "Rol başarıyla oluşturuldu" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Rol oluşturma başarısız: {ex.Message}" });
        }
    }

    /// <summary>
    /// SeedData'yı güvenle günceller - mevcut veriler korunur, sadece eksik veriler eklenir
    /// </summary>
    [HttpPost("update-seeddata")]
    public async Task<IActionResult> UpdateSeedData()
    {
        try
        {
            await SeedData.UpdateSeedDataAsync(_context);
            return Ok(new { success = true, message = "SeedData başarıyla güncellendi. Mevcut veriler korundu, sadece eksik veriler eklendi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"SeedData güncelleme başarısız: {ex.Message}" });
        }
    }
} 