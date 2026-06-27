using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PGManagement.Infrastructure.Repositories
{
    public class KycRepository : IKycRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public KycRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<int> CreateAsync(int tenantId, int bookingId, string documentType, string fileUrl)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_Kyc_Insert", new { TenantId = tenantId, BookingId = bookingId, DocumentType = documentType, FileUrl = fileUrl },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<KycDocumentResponse>> GetByTenantIdAsync(int tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<KycDocumentResponse>(
                "dbo.sp_Kyc_GetByTenantId", new { TenantId = tenantId }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStatusAsync(
    int id,
    string status,
    string ownerId,
    string? rejectionReason)
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                "dbo.sp_Kyc_UpdateStatus",
                new
                {
                    Id = id,
                    Status = status,
                    OwnerId = ownerId,
                    Reason = rejectionReason
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<KycDocumentResponse>>
    GetPendingByOwnerIdAsync(string ownerId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<KycDocumentResponse>(
                "dbo.sp_Kyc_GetByOwnerId",
                new { OwnerId = ownerId },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<KycDocumentResponse?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<KycDocumentResponse>(
                "SELECT * FROM KycDocument WHERE Id=@Id",
                new { Id = id });
        }

    }


}
