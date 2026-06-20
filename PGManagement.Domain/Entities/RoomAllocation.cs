using PGManagement.Domain.Common;

namespace PGManagement.Domain.Entities;

public class RoomAllocation : BaseEntity
{
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public DateTime AllocatedFrom { get; set; }
    public DateTime? AllocatedTo { get; set; }
    public bool IsActive { get; set; } = true;
}