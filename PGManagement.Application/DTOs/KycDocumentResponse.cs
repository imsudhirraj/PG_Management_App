namespace PGManagement.Application.DTOs;

public class KycDocumentResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}