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
            Longitude = request.Longitude,

            PropertyType = request.PropertyType,

            GenderType = request.GenderType,

            DepositAmount = request.DepositAmount,

            FoodAvailable = request.FoodAvailable,

            WifiAvailable = request.WifiAvailable,

            LaundryAvailable = request.LaundryAvailable,

            ParkingAvailable = request.ParkingAvailable,

            ACAvailable = request.ACAvailable,

            CoverImageUrl = request.CoverImageUrl
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
        var pg =
            await _pgRepository.GetByIdAsync(id);

        if (pg == null)
            return NotFound();

        return Ok(pg);
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchByLocation(
    [FromQuery] decimal lat,
    [FromQuery] decimal lng,
    [FromQuery] double radiusKm = 5)
    {
        var results =
            await _pgRepository.SearchByLocationAsync(
                lat,
                lng,
                radiusKm);

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

    [AllowAnonymous]
    [HttpGet("home")]
    public async Task<IActionResult> GetHomeData(
    [FromQuery] decimal lat,
    [FromQuery] decimal lng)
    {
        var nearby =
            await _pgRepository.SearchByLocationAsync(lat, lng, 5);

        return Ok(new
        {
            Featured = nearby.Where(x => x.Featured).Take(10),

            Nearby = nearby.Take(20),

            Recommended = nearby
                .OrderByDescending(x => x.Rating)
                .Take(10),

            RecentlyAdded = nearby
                .OrderByDescending(x => x.Id)
                .Take(10)
        });
    }

    [AllowAnonymous]
    [HttpGet("filters")]
    public IActionResult GetFilters()
    {
        return Ok(new
        {
            PropertyTypes = new[]
            {
            "PG",
            "Hostel",
            "CoLiving"
        },

            GenderTypes = new[]
            {
            "Boys",
            "Girls",
            "Unisex"
        },

            Amenities = new[]
            {
            "WiFi",
            "Food",
            "Laundry",
            "Parking",
            "AC"
        }
        });
    }

    [AllowAnonymous]
    [HttpGet("featured")]
    public async Task<IActionResult> Featured(
    [FromQuery] decimal lat,
    [FromQuery] decimal lng)
    {
        var result =
            await _pgRepository.SearchByLocationAsync(lat, lng, 10);

        return Ok(
            result.Where(x => x.Featured));
    }

    [AllowAnonymous]
    [HttpGet("search-by-text")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new List<PGCardResponse>());

        var results = await _pgRepository.SearchPGsAsync(query);
        return Ok(results);
        //return Ok(new { Count = results.Count(), Query = query, Results = results });

    }
}