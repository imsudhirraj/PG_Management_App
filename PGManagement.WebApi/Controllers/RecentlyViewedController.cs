using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.Interfaces;
using System.Security.Claims;
namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Tenant")]
public class RecentlyViewedController : ControllerBase
{
    private readonly IRecentlyViewedRepository _repository;
    public RecentlyViewedController(IRecentlyViewedRepository repository)
    {
        _repository = repository;
    }
    // Called whenever user opens PG Detail page
    [HttpPost("{pgId}")]
    public async Task<IActionResult> Add(int pgId)
    {
        var userId =
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _repository.AddAsync(userId, pgId);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> MyRecent()
    {
        var userId =
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(
        await _repository.GetByUserAsync(userId));
    }
}