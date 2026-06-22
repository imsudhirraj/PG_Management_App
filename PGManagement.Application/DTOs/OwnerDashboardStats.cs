using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class OwnerDashboardStats
{
    public int TotalPGs { get; set; }
    public int TotalRooms { get; set; }
    public int VacantRooms { get; set; }
    public int ActiveTenants { get; set; }
    public int TotalBeds { get; set; }
    public int OccupiedBeds { get; set; }
    public double OccupancyRate => TotalBeds == 0 ? 0 : Math.Round((double)OccupiedBeds / TotalBeds * 100, 1);
    public int PendingBookings { get; set; }
    public int OpenComplaints { get; set; }
    public decimal ThisMonthRevenue { get; set; }
}
