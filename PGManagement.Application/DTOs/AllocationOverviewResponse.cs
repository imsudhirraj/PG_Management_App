namespace PGManagement.Application.DTOs;

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
