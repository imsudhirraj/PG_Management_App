namespace PGManagement.Application.DTOs;

public class PGResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Address { get; set; } = "";

    public string City { get; set; } = "";

    public string? Description { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string? CoverImageUrl { get; set; }

    public decimal StartingRent { get; set; }

    public decimal DepositAmount { get; set; }

    public string PropertyType { get; set; } = "";

    public string GenderType { get; set; } = "";

    public bool Featured { get; set; }

    public bool Verified { get; set; }

    public decimal Rating { get; set; }

    public int ReviewCount { get; set; }

    public bool FoodAvailable { get; set; }

    public bool WifiAvailable { get; set; }

    public bool LaundryAvailable { get; set; }

    public bool ParkingAvailable { get; set; }

    public bool ACAvailable { get; set; }

    public List<PGImageResponse> Images { get; set; } = new();

    public List<PGAmenityResponse> Amenities { get; set; } = new();

    public List<RoomResponse> Rooms { get; set; } = new();

    public List<PGNearbyPlaceResponse> NearbyPlaces { get; set; } = new();

    public List<PGRuleResponse> Rules { get; set; } = new();

    public List<PGFAQResponse> FAQs { get; set; } = new();

    public List<PGReviewResponse> Reviews { get; set; } = new();
}