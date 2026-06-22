using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Domain.Entities;
using System.Data;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;


[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [Authorize]
    [HttpGet("claims")]
    public IActionResult GetMyClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}

[ApiController]
[Route("api/[controller]")]
public class PGController : ControllerBase
{
    private readonly IPGRepository _pgRepository;
    public PGController(IPGRepository pgRepository) => _pgRepository = pgRepository;

    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePGRequest request)
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var pg = new PG
        {
            OwnerId = ownerId,
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var id = await _pgRepository.CreateAsync(pg);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePGRequest request)
    {
        await _pgRepository.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Owner")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _pgRepository.SoftDeleteAsync(id);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pg = await _pgRepository.GetByIdAsync(id);
        return pg is null ? NotFound() : Ok(pg);
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchByLocation(
        [FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] double radiusKm = 5)
    {
        var results = await _pgRepository.SearchByLocationAsync(lat, lng, radiusKm);
        return Ok(results);
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("my-pgs")]
    public async Task<IActionResult> GetMyPGs()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var pgs = await _pgRepository.GetByOwnerIdAsync(ownerId);
        return Ok(pgs);
    }
}