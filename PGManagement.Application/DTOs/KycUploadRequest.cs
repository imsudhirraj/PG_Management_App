using Microsoft.AspNetCore.Http;

namespace PGManagement.Application.DTOs;

public class KycUploadRequest
{
    public int BookingId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public IFormFile File { get; set; } = null!;
}