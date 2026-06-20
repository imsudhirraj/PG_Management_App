using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;

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
}