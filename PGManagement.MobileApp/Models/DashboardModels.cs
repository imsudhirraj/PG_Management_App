namespace PGManagement.MobileApp.Models;

public class OwnerDashboardStats
{
    public int TotalPGs { get; set; }
    public int TotalRooms { get; set; }
    public int VacantRooms { get; set; }
    public int ActiveTenants { get; set; }
    public int TotalBeds { get; set; }
    public int OccupiedBeds { get; set; }
    public double OccupancyRate { get; set; }
}