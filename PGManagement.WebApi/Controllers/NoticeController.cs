using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoticeController : ControllerBase
{
    private readonly INoticeRepository _noticeRepository;
    public NoticeController(INoticeRepository noticeRepository) => _noticeRepository = noticeRepository;

    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoticeRequest request)
    {
        var id = await _noticeRepository.CreateAsync(request);
        return Ok(new { id });
    }

    [Authorize]
    [HttpGet("pg/{pgId}")]
    public async Task<IActionResult> GetByPGId(int pgId)
    {
        var notices = await _noticeRepository.GetByPGIdAsync(pgId);
        return Ok(notices);
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("by-owner")]
    public async Task<IActionResult> GetByOwner()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var notices = await _noticeRepository.GetByOwnerIdAsync(ownerId);
        return Ok(notices);
    }
}