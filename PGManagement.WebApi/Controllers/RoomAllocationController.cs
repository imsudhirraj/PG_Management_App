using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Exceptions;
using PGManagement.Application.Interfaces;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomAllocationController : ControllerBase
{
    private readonly IRoomAllocationRepository _allocationRepository;
    public RoomAllocationController(IRoomAllocationRepository allocationRepository) => _allocationRepository = allocationRepository;

    [Authorize(Roles = "Owner")]
    [HttpPost("allocate")]
    public async Task<IActionResult> Allocate([FromBody] AllocateRoomRequest request)
    {
        try
        {
            var id = await _allocationRepository.AllocateAsync(request);
            return Ok(new { id });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Owner")]
    [HttpPost("{allocationId}/vacate")]
    public async Task<IActionResult> Vacate(int allocationId)
    {
        await _allocationRepository.VacateAsync(allocationId);
        return NoContent();
    }
}