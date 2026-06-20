using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;

namespace PGManagement.WebApi.Controllers
{
     public class PaymentController : ControllerBase
        {
            private readonly IPaymentRepository _paymentRepository;
            public PaymentController(IPaymentRepository paymentRepository) => _paymentRepository = paymentRepository;

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
      }   
    
}
