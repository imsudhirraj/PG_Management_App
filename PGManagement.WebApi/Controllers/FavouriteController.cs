using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Tenant")]
public class FavouriteController : ControllerBase
{
    private readonly IFavouriteRepository _repository;

    public FavouriteController(IFavouriteRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("{pgId}")]
    public async Task<IActionResult> Add(int pgId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _repository.AddAsync(userId, pgId);

        return Ok();
    }

    [HttpDelete("{pgId}")]
    public async Task<IActionResult> Remove(int pgId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _repository.RemoveAsync(userId, pgId);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> MyWishlist()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        return Ok(await _repository.GetMyFavouritesAsync(userId));
    }

    [HttpGet("{pgId}/exists")]
    public async Task<IActionResult> Exists(int pgId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        return Ok(await _repository.ExistsAsync(userId, pgId));
    }
}