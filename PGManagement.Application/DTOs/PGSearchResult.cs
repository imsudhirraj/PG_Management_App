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
}