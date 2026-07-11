using PGManagement.Domain.Enums;
using System.Text.Json.Serialization;

public class CreateRoomRequest
{
    public int PGId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomStatus Status { get; set; }

    public int Floor { get; set; }

    public int TotalBeds { get; set; }

    public decimal RentAmount { get; set; }

    public decimal SecurityDeposit { get; set; }

    public string? Description { get; set; }
}