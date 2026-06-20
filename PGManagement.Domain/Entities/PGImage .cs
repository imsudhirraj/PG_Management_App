using PGManagement.Domain.Common;

namespace PGManagement.Domain.Entities;

public class PGImage : BaseEntity
{
    public int PGId { get; set; }
    public PG PG { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
}