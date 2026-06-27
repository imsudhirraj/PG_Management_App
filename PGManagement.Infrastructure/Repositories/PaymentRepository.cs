using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public PaymentRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<int> CreateAsync(int tenantId, RecordPaymentRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_Payment_Insert",
                new { TenantId = tenantId, request.Amount, request.Type, request.PaymentDate },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PaymentResponse>> GetByTenantIdAsync(int tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<PaymentResponse>(
                "dbo.sp_Payment_GetByTenantId",
                new { TenantId = tenantId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PaymentOverviewResponse>> GetByOwnerIdAsync(string ownerId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<PaymentOverviewResponse>(
                "dbo.sp_Payment_GetByOwnerId", new { OwnerId = ownerId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<PaymentDetailsResponse?> GetPaymentDetailsAsync(
    int bookingId)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<PaymentDetailsResponse>(
                "dbo.sp_Payment_GetPaymentDetails",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UploadAsync(
    int tenantId,
    PaymentUploadRequest request,
    string screenshotUrl)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_Payment_Insert",
                new
                {
                    TenantId = tenantId,

                    request.BookingId,

                    request.Amount,

                    request.Type,

                    request.TransactionId,

                    request.PaymentMethod,

                    ScreenshotUrl = screenshotUrl
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task VerifyAsync(
    int paymentId,
    string ownerId,
    VerifyPaymentRequest request)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                "dbo.sp_Payment_Verify",
                new
                {
                    Id = paymentId,

                    request.Status,

                    OwnerId = ownerId,

                    request.Remarks
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
