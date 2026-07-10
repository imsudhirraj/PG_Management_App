namespace PGManagement.Application.DTOs;

public class PGNearbyPlaceResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }
}