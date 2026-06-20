using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class AllocateRoomRequest
{
    public int RoomId { get; set; }
    public int TenantId { get; set; }
    public DateTime AllocatedFrom { get; set; }
}
