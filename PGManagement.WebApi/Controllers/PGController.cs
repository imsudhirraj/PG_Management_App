using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Domain.Entities;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PGController : ControllerBase
{
    private readonly IPGRepository _pgRepository;
    public PGController(IPGRepository pgRepository) => _pgRepository = pgRepository;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePGRequest request)
    {
        var pg = new PG
        {
            OwnerId = request.OwnerId,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pg = await _pgRepository.GetByIdAsync(id);
        return pg is null ? NotFound() : Ok(pg);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByLocation(
        [FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] double radiusKm = 5)
    {
        var results = await _pgRepository.SearchByLocationAsync(lat, lng, radiusKm);
        return Ok(results);
    }
}