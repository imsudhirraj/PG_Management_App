namespace PGManagement.MobileApp.Models;

public class PaymentDetailsResponse
{
    public int BookingId { get; set; }

    public string PGName { get; set; } = "";

    public string RoomNumber { get; set; } = "";

    public decimal Amount { get; set; }

    public string AccountHolderName { get; set; } = "";

    public string UpiId { get; set; } = "";
}