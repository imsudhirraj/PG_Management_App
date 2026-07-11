using System.Text.Json.Serialization;

namespace PGManagement.MobileApp.Models
{
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

        // Core UI Display Helpers
        public string DisplayImage => string.IsNullOrWhiteSpace(CoverImageUrl)
            ? "pg_placeholder.png"
            : CoverImageUrl;

        public string RatingText => $"{Rating:0.0} ({ReviewCount})";

        public string RentText => $"₹{StartingRent:N0}/month";

        public bool ShowFeatured => Featured;

        public bool ShowVerified => Verified;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        public DateTime? ViewedAt { get; set; }
    }
}