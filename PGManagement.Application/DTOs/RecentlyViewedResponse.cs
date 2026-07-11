namespace PGManagement.Application.DTOs;

public class RecentlyViewedResponse
{

    public int PGId { get; set; }
    public string PGName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal StartingRent { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime ViewedAt { get; set; }
}