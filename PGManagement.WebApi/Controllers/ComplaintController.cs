using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintController : ControllerBase
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly ITenantRepository _tenantRepository;

    public ComplaintController(IComplaintRepository complaintRepository, ITenantRepository tenantRepository)
    {
        _complaintRepository = complaintRepository;
        _tenantRepository = tenantRepository;
    }

    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<IActionResult> Raise([FromBody] RaiseComplaintRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tenant = await _tenantRepository.GetByUserIdAsync(userId);
        if (tenant is null)
            return BadRequest(new { message = "No tenant profile found for this account." });

        var id = await _complaintRepository.CreateAsync(tenant.Id, request);
        return Ok(new { id });
    }

    [Authorize(Roles = "Tenant")]
    [HttpGet("my-complaints")]
    public async Task<IActionResult> GetMyComplaints()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tenant = await _tenantRepository.GetByUserIdAsync(userId);
        if (tenant is null) return Ok(Array.Empty<ComplaintResponse>());

        var complaints = await _complaintRepository.GetByTenantIdAsync(tenant.Id);
        return Ok(complaints);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        await _complaintRepository.UpdateStatusAsync(id, request.Status);
        return NoContent();
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("by-owner")]
    public async Task<IActionResult> GetByOwner()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var complaints = await _complaintRepository.GetByOwnerIdAsync(ownerId);
        return Ok(complaints);
    }
}