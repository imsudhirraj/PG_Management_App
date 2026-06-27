namespace PGManagement.MobileApp.Models;

public class RoomResponse
{
    public int Id { get; set; }
    public int PGId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public int TotalBeds { get; set; }
    public decimal RentAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AvailableBeds { get; set; }
}

public class CreateBookingRequest   // 👈 ADD THIS
{
    public int RoomId { get; set; }
    public string? Message { get; set; }

}