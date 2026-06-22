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

        public async Task<int> CreateAsync(int tenantId, string documentType, string fileUrl)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_Kyc_Insert", new { TenantId = tenantId, DocumentType = documentType, FileUrl = fileUrl },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<KycDocumentResponse>> GetByTenantIdAsync(int tenantId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<KycDocumentResponse>(
                "dbo.sp_Kyc_GetByTenantId", new { TenantId = tenantId }, commandType: CommandType.StoredProcedure);
        }
    }
}
