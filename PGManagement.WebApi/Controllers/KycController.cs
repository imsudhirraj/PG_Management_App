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
    private readonly IWebHostEnvironment _env;
    private readonly IKycRepository _kycRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IFileStorageService _fileStorageService;

    public KycController(IWebHostEnvironment env, IKycRepository kycRepository, ITenantRepository tenantRepository, IFileStorageService fileStorageService)
    {
        _env = env;
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

        if (tenant == null)
            return BadRequest("Tenant not found.");

        var fileUrl =
            await _fileStorageService.SaveFileAsync(
                request.File,
                "kyc");

        var id = await _kycRepository.CreateAsync(
            tenant.Id,
            request.BookingId,
            request.DocumentType,
            fileUrl);

        return Ok(new
        {
            id,
            fileUrl
        });
    }

    [Authorize]
    [HttpGet("tenant/{tenantId}")]
    public async Task<IActionResult> GetByTenant(int tenantId)
    {
        var docs = await _kycRepository.GetByTenantIdAsync(tenantId);
        return Ok(docs);
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("by-owner")]
    public async Task<IActionResult> GetByOwner()
    {
        var ownerId = User.FindFirstValue(
            ClaimTypes.NameIdentifier)!;

        var docs =
            await _kycRepository.GetPendingByOwnerIdAsync(ownerId);

        return Ok(docs);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}/verify")]
    public async Task<IActionResult> Verify(
    int id,
    [FromBody] VerifyKycRequest request)
    {
        var ownerId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _kycRepository.UpdateStatusAsync(
            id,
            request.Status,
            ownerId,
            request.Remarks);

        return NoContent();
    }
    [Authorize(Roles = "Owner,Tenant")]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var doc = await _kycRepository.GetByIdAsync(id);

        if (doc == null)
            return NotFound();

        var fileName = Path.GetFileName(doc.FileUrl);

        var filePath = Path.Combine(
            _env.WebRootPath,
            "uploads",
            "kyc",
            fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found.");

        return PhysicalFile(
            filePath,
            "application/pdf",
            enableRangeProcessing: true);
    }

}