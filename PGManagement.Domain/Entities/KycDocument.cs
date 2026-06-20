using PGManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Domain.Entities
{
    public class KycDocument : BaseEntity
    {
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public string DocumentType { get; set; } = string.Empty; // Aadhaar, PAN, etc.
        public string FileUrl { get; set; } = string.Empty;
    }
}
