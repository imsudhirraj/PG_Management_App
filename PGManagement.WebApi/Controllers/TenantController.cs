using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;
    public TenantController(ITenantRepository tenantRepository) => _tenantRepository = tenantRepository;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        var id = await _tenantRepository.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUserId), new { userId = request.UserId }, new { id });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var tenant = await _tenantRepository.GetByUserIdAsync(userId);
        return tenant is null ? NotFound() : Ok(tenant);
    }
}