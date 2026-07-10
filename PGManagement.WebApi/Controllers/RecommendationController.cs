using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationRepository _repository;

    public RecommendationController(
        IRecommendationRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Featured PGs
    /// </summary>
    [AllowAnonymous]
    [HttpGet("featured")]
    public async Task<IActionResult> Featured()
    {
        var result =
            await _repository.GetFeaturedAsync();

        return Ok(result);
    }

    /// <summary>
    /// Nearby PGs
    /// </summary>
    [AllowAnonymous]
    [HttpGet("nearby")]
    public async Task<IActionResult> Nearby(
        decimal latitude,
        decimal longitude,
        double radiusKm = 5)
    {
        var result =
            await _repository.GetNearbyAsync(
                latitude,
                longitude,
                radiusKm);

        return Ok(result);
    }

    /// <summary>
    /// Personalized Recommendation
    /// </summary>
    [Authorize(Roles = "Tenant")]
    [HttpGet("recommended")]
    public async Task<IActionResult> Recommended()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

        var result =
            await _repository.GetRecommendedAsync(
                userId);

        return Ok(result);
    }

    /// <summary>
    /// Recently Added PGs
    /// </summary>
    [AllowAnonymous]
    [HttpGet("recent")]
    public async Task<IActionResult> Recent()
    {
        var result =
            await _repository.GetRecentAsync();

        return Ok(result);
    }
}