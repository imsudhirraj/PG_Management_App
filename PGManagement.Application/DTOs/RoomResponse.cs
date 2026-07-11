using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class RoomResponse
{
    public int Id { get; set; }

    public int PGId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string RoomType { get; set; } = string.Empty;

    public int Floor { get; set; }

    public int TotalBeds { get; set; }

    public int AvailableBeds { get; set; }

    public decimal RentAmount { get; set; }

    public decimal SecurityDeposit { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Cover image for room card
    public string? CoverImageUrl { get; set; }

    // Premium additions
    public List<RoomImageResponse> Images { get; set; } = new();

    public List<RoomAmenityResponse> Amenities { get; set; } = new();
}