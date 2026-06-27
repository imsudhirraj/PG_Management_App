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

    public async Task<int> CreateAsync(string userId, CreateTenantRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Tenant_Insert",
            new { UserId = userId, request.FullName, request.Phone },
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

    public async Task<IEnumerable<TenantWithRoomResponse>> GetUnallocatedByOwnerIdAsync(string ownerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<TenantWithRoomResponse>(
            "dbo.sp_Tenant_GetUnallocatedByOwnerId",
            new { OwnerId = ownerId },
            commandType: CommandType.StoredProcedure
        );
    }
    public async Task<IEnumerable<TenantWithRoomResponse>> GetByOwnerIdAsync(string ownerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<TenantWithRoomResponse>(
            "dbo.sp_Tenant_GetByOwnerId", new { OwnerId = ownerId }, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(int id, UpdateTenantRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("dbo.sp_Tenant_Update",
            new { Id = id, request.FullName, request.Phone, request.EmergencyContactName, request.EmergencyContactPhone },
            commandType: CommandType.StoredProcedure);
    }

    public async Task CheckoutAsync(int tenantId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("dbo.sp_Tenant_Checkout", new { TenantId = tenantId }, commandType: CommandType.StoredProcedure);
    }
}