using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    public RoomController(IRoomRepository roomRepository) => _roomRepository = roomRepository;

    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        var id = await _roomRepository.CreateAsync(request);
        return CreatedAtAction(nameof(GetByPGId), new { pgId = request.PGId }, new { id });
    }

    [AllowAnonymous]
    [HttpGet("pg/{pgId}")]
    public async Task<IActionResult> GetByPGId(int pgId)
    {
        var rooms = await _roomRepository.GetByPGIdAsync(pgId);
        return Ok(rooms);
    }
}