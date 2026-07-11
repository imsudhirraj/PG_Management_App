namespace PGManagement.Application.DTOs;

public class NearbyRecommendationRequest
{
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public double RadiusKm { get; set; } = 5;
}