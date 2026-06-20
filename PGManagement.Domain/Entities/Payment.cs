using PGManagement.Domain.Common;
using PGManagement.Domain.Enums;

namespace PGManagement.Domain.Entities;

public class Payment : BaseEntity
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentType Type { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; }
}