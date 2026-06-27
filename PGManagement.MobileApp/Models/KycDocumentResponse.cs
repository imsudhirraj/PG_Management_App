namespace PGManagement.MobileApp.Models;

public class KycDocumentResponse
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public bool CanVerify =>
        Status == "Pending";
}