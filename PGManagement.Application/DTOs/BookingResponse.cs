using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class BookingResponse
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string PGName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }

    public bool CanUploadKyc =>
    Status == "Approved";

    public bool CanPay =>
        Status == "Approved";

    public bool CanChat =>
        Status == "Approved";
}
