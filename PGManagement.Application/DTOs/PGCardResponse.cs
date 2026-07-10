namespace PGManagement.Application.DTOs;

public class PGCardResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal StartingRent { get; set; }

    public decimal DepositAmount { get; set; }

    public decimal Rating { get; set; }

    public int ReviewCount { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool Featured { get; set; }

    public bool Verified { get; set; }

    public string PropertyType { get; set; } = string.Empty;

    public string GenderType { get; set; } = string.Empty;

    public double DistanceKm { get; set; }

    public int AvailableBeds { get; set; }
}