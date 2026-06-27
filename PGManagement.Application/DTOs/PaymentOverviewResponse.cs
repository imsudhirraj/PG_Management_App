using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs
{
    public class PaymentOverviewResponse
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; } = "";

        public string Email { get; set; } = "";

        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime PaymentDate { get; set; }

        public string TransactionId { get; set; } = "";

        public string PaymentMethod { get; set; } = "";

        public string ScreenshotUrl { get; set; } = "";

        public string? Remarks { get; set; }

        public string RoomNumber { get; set; } = "";

        public int PGId { get; set; }

        public string PGName { get; set; } = "";

        public string Address { get; set; } = "";

        public string City { get; set; }
    }
}
