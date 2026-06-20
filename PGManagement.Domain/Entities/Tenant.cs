using PGManagement.Domain.Common;

namespace PGManagement.Domain.Entities;

public class Tenant : BaseEntity
{
    public string UserId { get; set; } = string.Empty; // FK to ApplicationUser.Id
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }

    public ICollection<RoomAllocation> RoomAllocations { get; set; } = new List<RoomAllocation>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();
}