namespace PGManagement.Application.DTOs;

public class PGReviewResponse
{
    public int Id { get; set; }

    public string TenantName { get; set; } = "";

    public decimal Rating { get; set; }

    public string Review { get; set; } = "";

    public DateTime CreatedOn { get; set; }
}