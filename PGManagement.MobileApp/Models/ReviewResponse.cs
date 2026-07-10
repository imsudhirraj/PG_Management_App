using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Models
{
    public class ReviewResponse
    {
        public int Id { get; set; }

        public int PGId { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; } = string.Empty;

        public decimal Rating { get; set; }

        public string? ReviewText { get; set; }

        public string? OwnerReply { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
