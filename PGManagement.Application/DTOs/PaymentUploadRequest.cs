using Microsoft.AspNetCore.Http;

namespace PGManagement.Application.DTOs;

public class PaymentUploadRequest
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = "Advance";

    public string TransactionId { get; set; } = "";

    public string PaymentMethod { get; set; } = "UPI";

    public IFormFile Screenshot { get; set; } = default!;
}