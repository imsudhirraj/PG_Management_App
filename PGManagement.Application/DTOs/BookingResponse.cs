public class BookingResponse
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int PGId { get; set; }

    public int RoomId { get; set; }

    public string PGName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public decimal RentAmount { get; set; }

    public DateTime BookingDate { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string? Message { get; set; }

    public bool CanUploadKyc => Status == "Approved";

    public bool CanPay =>
        Status == "Approved" &&
        PaymentStatus != "Paid";

    public bool CanChat => Status == "Approved";
}