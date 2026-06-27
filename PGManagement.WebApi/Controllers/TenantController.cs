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

    [Authorize(Roles = "Owner")]
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

    [Authorize(Roles = "Owner")]
    [HttpGet("by-owner")]
    public async Task<IActionResult> GetByOwner()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tenants = await _tenantRepository.GetByOwnerIdAsync(ownerId);
        return Ok(tenants);
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("unallocated")]
    public async Task<IActionResult> GetUnallocatedTenants()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var unallocatedTenants = await _tenantRepository.GetUnallocatedByOwnerIdAsync(ownerId);
        return Ok(unallocatedTenants);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTenantRequest request)
    {
        await _tenantRepository.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Owner")]
    [HttpPost("{id}/checkout")]
    public async Task<IActionResult> Checkout(int id)
    {
        await _tenantRepository.CheckoutAsync(id);
        return NoContent();
    }
}