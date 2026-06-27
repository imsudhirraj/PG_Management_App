namespace PGManagement.MobileApp.Models;

public class AllocationOverviewResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public DateTime AllocatedFrom { get; set; }
    public DateTime? AllocatedTo { get; set; }
    public bool IsActive { get; set; }
}

public class PaymentOverviewResponse
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string TenantName { get; set; } = "";

    public string Email { get; set; } = "";

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = "";

    public string Status { get; set; } = "";

    public DateTime PaymentDate { get; set; }

    public string TransactionId { get; set; } = "";

    public string PaymentMethod { get; set; } = "";

    public string ScreenshotUrl { get; set; } = "";

    public string? Remarks { get; set; }

    public string RoomNumber { get; set; } = "";

    public string PGName { get; set; } = "";

    public string Address { get; set; } = "";

    public string City { get; set; } = "";
}

public class ComplaintOverviewResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RaisedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class BookingOverviewResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int PGId { get; set; }
    public string PGName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class NoticeOverviewResponse
{
    public int Id { get; set; }
    public int PGId { get; set; }
    public string PGName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}