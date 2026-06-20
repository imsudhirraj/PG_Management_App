using PGManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Domain.Entities
{
    public class Notice : BaseEntity
    {
        public int PGId { get; set; }
        public PG PG { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

}
