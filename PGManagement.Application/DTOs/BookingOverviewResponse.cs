using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs
{
    public class BookingOverviewResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int PGId { get; set; }
        public string PGName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CanApprove => Status == "Pending";
        public string? Message { get; set; }
    }
}
