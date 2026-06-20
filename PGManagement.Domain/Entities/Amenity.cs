using PGManagement.Domain.Common;

namespace PGManagement.Domain.Entities;

public class Amenity : BaseEntity
{
    public string Name { get; set; } = string.Empty; // WiFi, Laundry, AC, etc.
    public ICollection<PGAmenity> PGAmenities { get; set; } = new List<PGAmenity>();
}

public class PGAmenity
{
    public int PGId { get; set; }
    public PG PG { get; set; } = null!;
    public int AmenityId { get; set; }
    public Amenity Amenity { get; set; } = null!;
}