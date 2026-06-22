using Microsoft.AspNetCore.Http;

namespace PGManagement.Application.DTOs;

public class KycUploadRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public IFormFile File { get; set; } = null!;
}