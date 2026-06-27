using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.WebApi.Services;
using System.Security.Claims;

namespace PGManagement.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
        {
            private readonly IPaymentRepository _paymentRepository;
        private readonly ITenantRepository _tenantRepository;

        private readonly IFileStorageService _fileStorageService;

        public PaymentController(
    IPaymentRepository paymentRepository,
    ITenantRepository tenantRepository,
    IFileStorageService fileStorageService)
        {
            _paymentRepository = paymentRepository;

            _tenantRepository = tenantRepository;

            _fileStorageService = fileStorageService;
        }

            [Authorize(Roles = "Owner")]
            [HttpPost("tenant/{tenantId}")]
            public async Task<IActionResult> Record(int tenantId, [FromBody] RecordPaymentRequest request)
            {
                var id = await _paymentRepository.CreateAsync(tenantId, request);
                return Ok(new { id });
            }

            [Authorize]
            [HttpGet("tenant/{tenantId}")]
            public async Task<IActionResult> GetByTenantId(int tenantId)
            {
                var payments = await _paymentRepository.GetByTenantIdAsync(tenantId);
                return Ok(payments);
            }

        [Authorize(Roles = "Owner")]
        [HttpGet("by-owner")]
        public async Task<IActionResult> GetByOwner()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var payments = await _paymentRepository.GetByOwnerIdAsync(ownerId);
            return Ok(payments);
        }

        [Authorize(Roles = "Tenant")]
        [HttpPost]
        public async Task<IActionResult> Upload(
    [FromForm] PaymentUploadRequest request)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var tenant =
                await _tenantRepository.GetByUserIdAsync(userId);

            if (tenant == null)
                return BadRequest();

            var screenshotUrl =
                await _fileStorageService.SaveFileAsync(
                    request.Screenshot,
                    "payments");

            var id =
                await _paymentRepository.UploadAsync(
                    tenant.Id,
                    request,
                    screenshotUrl);

            return Ok(new
            {
                id,

                screenshotUrl
            });
        }

        [Authorize(Roles = "Tenant")]
        [HttpGet("details/{bookingId}")]
        public async Task<IActionResult> GetPaymentDetails(
    int bookingId)
        {
            var payment =
                await _paymentRepository.GetPaymentDetailsAsync(
                    bookingId);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}/verify")]
        public async Task<IActionResult> Verify(
    int id,
    VerifyPaymentRequest request)
        {
            var ownerId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _paymentRepository.VerifyAsync(
                id,
                ownerId,
                request);

            return Ok();
        }
    } 

    
}
