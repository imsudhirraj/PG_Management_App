using PGManagement.Domain.Common;
using PGManagement.Domain.Enums;

namespace PGManagement.Domain.Entities;

public class Booking : BaseEntity
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}