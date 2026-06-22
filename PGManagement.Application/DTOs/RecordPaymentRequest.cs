using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.DTOs;

public class RecordPaymentRequest
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // Rent, Deposit
    public DateTime PaymentDate { get; set; }
}
