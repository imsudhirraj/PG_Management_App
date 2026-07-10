using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Models
{
    public class CreateReviewRequest
    {
        public int PGId { get; set; }

        public decimal Rating { get; set; }

        public string? ReviewText { get; set; }
    }
}
