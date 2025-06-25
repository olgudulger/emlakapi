using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Abstract;
using Business.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IAuthService _authService;

    public DebugController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Test login - Admin kullanıcısıyla login yapar ve token detaylarını gösterir
    /// </summary>
    [HttpPost("test-login")]
    [AllowAnonymous]
    public async Task<IActionResult> TestLogin()
    {
        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "Admin123!"
        };

        var result = await _authService.LoginAsync(loginDto);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        // Token'ı decode edelim
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(result.Token);
        
        var tokenInfo = new
        {
            Token = result.Token,
            Claims = jsonToken.Claims.Select(c => new { c.Type, c.Value }).ToList(),
            Expires = jsonToken.ValidTo,
            Issuer = jsonToken.Issuer,
            Audience = jsonToken.Audiences.FirstOrDefault()
        };

        return Ok(new { 
            success = true, 
            authResult = result, 
            tokenDetails = tokenInfo 
        });
    }

    /// <summary>
    /// Token ile current user bilgilerini döner
    /// </summary>
    [HttpGet("current-user")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userInfo = new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Name = User.Identity?.Name,
            AuthenticationType = User.Identity?.AuthenticationType,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
            NameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value
        };

        return Ok(new { success = true, userInfo });
    }

    /// <summary>
    /// Authorization header'ını kontrol eder
    /// </summary>
    [HttpGet("check-auth-header")]
    [AllowAnonymous]
    public IActionResult CheckAuthHeader()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
        var isBearerToken = authHeader.StartsWith("Bearer ");
        
        var info = new
        {
            HasAuthorizationHeader = hasAuthHeader,
            AuthorizationHeader = authHeader,
            IsBearerFormat = isBearerToken,
            Token = isBearerToken ? authHeader.Substring(7) : null
        };

        return Ok(new { success = true, info });
    }

    /// <summary>
    /// Manual token verify
    /// </summary>
    [HttpPost("verify-token")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyToken([FromBody] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token gereklidir");

        var isValid = await _authService.VerifyTokenAsync(token);
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            return Ok(new
            {
                success = true,
                isValid,
                tokenDetails = new
                {
                    Claims = jsonToken.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                    Expires = jsonToken.ValidTo,
                    IsExpired = jsonToken.ValidTo < DateTime.UtcNow,
                    Issuer = jsonToken.Issuer,
                    Audience = jsonToken.Audiences.FirstOrDefault()
                }
            });
        }
        catch (Exception ex)
        {
            return Ok(new { success = true, isValid = false, error = ex.Message });
        }
    }
} 