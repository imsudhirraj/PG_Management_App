using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public TenantRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(CreateTenantRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Tenant_Insert",
            new { request.UserId, request.FullName, request.Phone },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<TenantResponse?> GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TenantResponse>(
            "dbo.sp_Tenant_GetByUserId",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }
}