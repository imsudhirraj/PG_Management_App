using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Models
{
    public class FavouriteResponse
    {
        public int Id { get; set; }

        public int PGId { get; set; }

        public string PGName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public decimal StartingRent { get; set; }

        public decimal Rating { get; set; }

        public int ReviewCount { get; set; }

        public string? CoverImageUrl { get; set; }

        public bool Featured { get; set; }

        public bool Verified { get; set; }
    }
}
