using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Abstract;
using Business.DTOs.Customer;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        
        if (customer == null)
            return NotFound($"ID {id} ile müşteri bulunamadı");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto customerCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var customer = await _customerService.CreateAsync(customerCreateDto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest($"Müşteri oluşturulurken hata: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto customerUpdateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await _customerService.UpdateAsync(id, customerUpdateDto);
        
        if (customer == null)
            return NotFound($"ID {id} ile müşteri bulunamadı");

        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        
        if (!result)
            return NotFound($"ID {id} ile müşteri bulunamadı");

        return Ok(new { message = "Müşteri başarıyla silindi" });
    }

    [HttpGet("by-interest-type/{interestType}")]
    public async Task<IActionResult> GetByInterestType(string interestType)
    {
        var customers = await _customerService.GetByInterestTypeAsync(interestType);
        return Ok(customers);
    }

    [HttpGet("by-customer-type/{customerType}")]
    public async Task<IActionResult> GetByCustomerType(string customerType)
    {
        var customers = await _customerService.GetByCustomerTypeAsync(customerType);
        return Ok(customers);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return BadRequest("Arama terimi gereklidir");

        var customers = await _customerService.SearchAsync(searchTerm);
        return Ok(customers);
    }

    [HttpPost("{customerId}/province-preferences/{provinceId}")]
    public async Task<IActionResult> AddProvincePreference(int customerId, int provinceId)
    {
        var result = await _customerService.AddProvincePreferenceAsync(customerId, provinceId);
        
        if (!result)
            return BadRequest("İl tercihi eklenemedi");

        return Ok(new { message = "İl tercihi başarıyla eklendi" });
    }

    [HttpDelete("{customerId}/province-preferences/{provinceId}")]
    public async Task<IActionResult> RemoveProvincePreference(int customerId, int provinceId)
    {
        var result = await _customerService.RemoveProvincePreferenceAsync(customerId, provinceId);
        
        if (!result)
            return NotFound("İl tercihi bulunamadı");

        return Ok(new { message = "İl tercihi başarıyla silindi" });
    }
} 