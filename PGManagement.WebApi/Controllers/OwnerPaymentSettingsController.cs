using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner")]
public class OwnerPaymentSettingsController : ControllerBase
{
    private readonly IOwnerPaymentSettingsRepository _repository;

    public OwnerPaymentSettingsController(
        IOwnerPaymentSettingsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ownerId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var data =
            await _repository.GetAsync(ownerId);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Save(
        OwnerPaymentSettingsRequest request)
    {
        var ownerId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _repository.SaveAsync(ownerId, request);

        return Ok(new
        {
            message = "Payment settings saved successfully."
        });
    }
}