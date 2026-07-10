public class ReviewResponse
{
    public int Id { get; set; }

    public int PGId { get; set; }

    public int TenantId { get; set; }

    public string TenantName { get; set; } = "";

    public int Rating { get; set; }

    public string? Review { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? OwnerReply { get; set; }

    public DateTime? ReplyDate { get; set; }
}