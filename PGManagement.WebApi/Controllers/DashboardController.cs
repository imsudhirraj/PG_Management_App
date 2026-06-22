using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardRepository _dashboardRepository;
    public DashboardController(IDashboardRepository dashboardRepository) => _dashboardRepository = dashboardRepository;

    [Authorize(Roles = "Owner")]
    [HttpGet("owner")]
    public async Task<IActionResult> GetOwnerStats()
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var stats = await _dashboardRepository.GetOwnerStatsAsync(ownerId);
        return Ok(stats);
    }
}