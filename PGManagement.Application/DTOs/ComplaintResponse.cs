using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs
{
    public class ComplaintResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RaisedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
