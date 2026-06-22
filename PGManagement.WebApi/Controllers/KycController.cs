using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.WebApi.Services;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KycController : ControllerBase
{
    private readonly IKycRepository _kycRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IFileStorageService _fileStorageService;

    public KycController(IKycRepository kycRepository, ITenantRepository tenantRepository, IFileStorageService fileStorageService)
    {
        _kycRepository = kycRepository;
        _tenantRepository = tenantRepository;
        _fileStorageService = fileStorageService;
    }

    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] KycUploadRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tenant = await _tenantRepository.GetByUserIdAsync(userId);
        if (tenant is null) return BadRequest(new { message = "No tenant profile found." });

        var fileUrl = await _fileStorageService.SaveFileAsync(request.File, "kyc");
        var id = await _kycRepository.CreateAsync(tenant.Id, request.DocumentType, fileUrl);
        return Ok(new { id, fileUrl });
    }

    [Authorize]
    [HttpGet("tenant/{tenantId}")]
    public async Task<IActionResult> GetByTenant(int tenantId)
    {
        var docs = await _kycRepository.GetByTenantIdAsync(tenantId);
        return Ok(docs);
    }
}