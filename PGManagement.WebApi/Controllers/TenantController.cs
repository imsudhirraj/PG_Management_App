using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;
    public TenantController(ITenantRepository tenantRepository) => _tenantRepository = tenantRepository;

    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var id = await _tenantRepository.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetByUserId), new { userId }, new { id });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var tenant = await _tenantRepository.GetByUserIdAsync(userId);
        return tenant is null ? NotFound() : Ok(tenant);
    }
}