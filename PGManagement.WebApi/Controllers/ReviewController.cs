using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _repository;

    public ReviewController(
        IReviewRepository repository)
    {
        _repository = repository;
    }

    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateReviewRequest request)
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

        var id =
            await _repository.CreateAsync(
                userId,
                request);

        return Ok(new { id });
    }

    [AllowAnonymous]
    [HttpGet("pg/{pgId}")]
    public async Task<IActionResult> GetReviews(
        int pgId)
    {
        var reviews =
            await _repository.GetByPGIdAsync(pgId);

        return Ok(reviews);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id}/reply")]
    public async Task<IActionResult> Reply(
        int id,
        OwnerReplyRequest request)
    {
        await _repository.ReplyAsync(
            id,
            request.Reply);

        return NoContent();
    }
}