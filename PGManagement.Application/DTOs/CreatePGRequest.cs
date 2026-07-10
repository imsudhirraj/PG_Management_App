namespace PGManagement.Application.DTOs;

public class CreatePGRequest
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    // Premium Fields

    public string PropertyType { get; set; } = "PG";

    public string GenderType { get; set; } = "Unisex";

    public decimal DepositAmount { get; set; }

    public bool FoodAvailable { get; set; }

    public bool WifiAvailable { get; set; }

    public bool LaundryAvailable { get; set; }

    public bool ParkingAvailable { get; set; }

    public bool ACAvailable { get; set; }

    public string? CoverImageUrl { get; set; }
    public bool Featured { get; set; }

    public bool Verified { get; set; }
}