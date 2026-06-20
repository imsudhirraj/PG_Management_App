using PGManagement.Domain.Common;

namespace PGManagement.Domain.Entities;

public class Complaint : BaseEntity
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime? ResolvedAt { get; set; }
}