using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class RecordPaymentRequest
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = "";

    public string TransactionId { get; set; } = "";

    public string PaymentMethod { get; set; } = "";
    public DateTime PaymentDate { get; set; }
}
