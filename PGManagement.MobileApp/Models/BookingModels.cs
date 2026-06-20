namespace PGManagement.MobileApp.Models;

public class BookingResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string PGName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
}