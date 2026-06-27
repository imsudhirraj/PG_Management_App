using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITenantRepository _tenantRepository;

    public BookingController(IBookingRepository bookingRepository, ITenantRepository tenantRepository)
    {
        _bookingRepository = bookingRepository;
        _tenantRepository = tenantRepository;
    }

    //[Authorize(Roles = "Tenant")]
    //[HttpPost]
    //public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    //    var tenant = await _tenantRepository.GetByUserIdAsync(userId);
    //    if (tenant is null)
    //        return BadRequest(new { message = "No tenant profile found for this account. Complete tenant registration first." });

    //    var id = await _bookingRepository.CreateAsync(tenant.Id, request);
    //    return CreatedAtAction(nameof(GetById), new { id }, new { id });
    //}

    [Authorize(Roles = "Tenant")]
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var bookings = await _bookingRepository.GetByUserIdAsync(userId);

        return Ok(bookings);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        return booking is null ? NotFound() : Ok(booking);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
    int id,
    [FromBody] UpdateBookingStatusRequest request)
    {
        if (request.Status == "Confirmed")
        {
            await _bookingRepository.ApproveBookingAsync(id);
        }
        else
        {
            await _bookingRepository.UpdateStatusAsync(id, request.Status);
        }

        return NoContent();
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("by-owner")]
    public async Task<IActionResult> GetByOwner()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bookings = await _bookingRepository.GetByOwnerIdAsync(ownerId);
        return Ok(bookings);
    }

    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] CreateBookingRequest request)
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

        var id =
            await _bookingRepository.CreateAsync(
                userId,
                request);

        return Ok(new { id });
    }

}