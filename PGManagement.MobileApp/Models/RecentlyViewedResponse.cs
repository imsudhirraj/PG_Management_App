using System;
using System.Text.Json.Serialization;

namespace PGManagement.MobileApp.Models
{
    public class RecentlyViewedResponse
    {
        [JsonPropertyName("pgId")] // match your exact backend JSON property key
        public int PGId { get; set; }

        [JsonPropertyName("pgName")] // if your API sends "name" instead, change this to "name"
        public string PGName { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("startingRent")] // if your API sends "rent", change this to "rent"
        public decimal StartingRent { get; set; }

        [JsonPropertyName("rating")]
        public decimal Rating { get; set; }

        [JsonPropertyName("reviewCount")]
        public int ReviewCount { get; set; }

        [JsonPropertyName("coverImageUrl")]
        public string? CoverImageUrl { get; set; }

        [JsonPropertyName("viewedAt")]
        public DateTime ViewedAt { get; set; }
    }
}