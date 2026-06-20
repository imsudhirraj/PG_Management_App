using PGManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> CreateAsync(int tenantId, RecordPaymentRequest request);
        Task<IEnumerable<PaymentResponse>> GetByTenantIdAsync(int tenantId);
    }
}
