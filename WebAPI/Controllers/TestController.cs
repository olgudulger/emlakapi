using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Test endpoint - Authorization gerektirmez
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicTest()
    {
        return Ok(new { message = "Bu endpoint herkese açık", timestamp = DateTime.Now });
    }

    /// <summary>
    /// Test endpoint - Authorization gerektirir
    /// </summary>
    [HttpGet("private")]
    [Authorize]
    public IActionResult PrivateTest()
    {
        var user = User.Identity?.Name ?? "Bilinmeyen";
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new 
        { 
            message = "Bu endpoint sadece authenticate kullanıcılar için",
            user = user,
            claims = claims,
            timestamp = DateTime.Now 
        });
    }

    /// <summary>
    /// Admin test endpoint
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminTest()
    {
        return Ok(new { message = "Bu endpoint sadece Admin rolü için", timestamp = DateTime.Now });
    }
} 