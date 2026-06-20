using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class CreateRoomRequest
{
    public int PGId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty; // Single, Double, Triple, Dormitory
    public int TotalBeds { get; set; }
    public decimal RentAmount { get; set; }
}
