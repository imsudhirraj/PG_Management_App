using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class RoomResponse
{
    public int Id { get; set; }
    public int PGId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public int TotalBeds { get; set; }
    public decimal RentAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AvailableBeds { get; set; }
}