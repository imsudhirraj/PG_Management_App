using PGManagement.Domain.Common;
using PGManagement.Domain.Enums;

namespace PGManagement.Domain.Entities;

public class Room : BaseEntity
{
    public int PGId { get; set; }
    public PG PG { get; set; } = null!;

    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public int TotalBeds { get; set; }
    public decimal RentAmount { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Vacant;

    public ICollection<RoomAllocation> Allocations { get; set; } = new List<RoomAllocation>();

    // Computed, not mapped — available beds = TotalBeds - active allocations
    public int AvailableBeds => TotalBeds - Allocations.Count(a => a.IsActive);
}