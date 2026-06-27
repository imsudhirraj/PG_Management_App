public class KycDocumentResponse
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string TenantName { get; set; } = "";

    public string Email { get; set; } = "";

    public int BookingId { get; set; }

    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = "";

    public int PGId { get; set; }

    public string PGName { get; set; } = "";

    public string Address { get; set; } = "";

    public string City { get; set; } = "";

    public string DocumentType { get; set; } = "";

    public string FileUrl { get; set; } = "";

    public DateTime UploadedAt { get; set; }

    public string VerificationStatus { get; set; } = "";

    public DateTime? VerifiedAt { get; set; }

    public string? VerifiedByOwnerId { get; set; }

    public string? RejectionReason { get; set; }
}