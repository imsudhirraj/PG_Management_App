using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Domain.Entities;

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
        var room = new Room
        {
            PGId = request.PGId,
            RoomNumber = request.RoomNumber,
            RoomType = request.RoomType,
            Floor = request.Floor,
            TotalBeds = request.TotalBeds,
            RentAmount = request.RentAmount,
            SecurityDeposit = request.SecurityDeposit,
            Description = request.Description,
            Status = request.Status
        };

        var id = await _roomRepository.CreateAsync(room);

        return CreatedAtAction(
            nameof(GetByPGId),
            new { pgId = request.PGId },
            new { id });
    }

    [AllowAnonymous]
    [HttpGet("pg/{pgId}")]
    public async Task<IActionResult> GetByPGId(int pgId)
    {
        var rooms = await _roomRepository.GetByPGIdAsync(pgId);
        return Ok(rooms);
    }
}