namespace PGManagement.Application.DTOs;

public class PGSearchResult
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public double DistanceKm { get; set; }

    public int AvailableBeds { get; set; }

    // Premium additions

    public decimal StartingRent { get; set; }

    public decimal DepositAmount { get; set; }

    public string PropertyType { get; set; } = string.Empty;

    public string GenderType { get; set; } = string.Empty;

    public bool Featured { get; set; }

    public bool Verified { get; set; }

    public decimal Rating { get; set; }

    public int ReviewCount { get; set; }

    public string? CoverImageUrl { get; set; }
}