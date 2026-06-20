using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public ComplaintRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(int tenantId, RaiseComplaintRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Complaint_Insert",
            new { TenantId = tenantId, request.Title, request.Description },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ComplaintResponse>> GetByTenantIdAsync(int tenantId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ComplaintResponse>(
            "dbo.sp_Complaint_GetByTenantId",
            new { TenantId = tenantId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "dbo.sp_Complaint_UpdateStatus",
            new { Id = id, Status = status },
            commandType: CommandType.StoredProcedure);
    }
}