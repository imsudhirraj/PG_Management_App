using Dapper;
using Microsoft.Data.SqlClient;
using PGManagement.Application.DTOs;
using PGManagement.Application.Exceptions;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class RoomAllocationRepository : IRoomAllocationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public RoomAllocationRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> AllocateAsync(AllocateRoomRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        try
        {
            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_RoomAllocation_Allocate",
                new { request.RoomId, request.TenantId, request.AllocatedFrom },
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Message.Contains("No beds available"))
        {
            throw new BusinessRuleException(ex.Message);
        }
    }

    public async Task VacateAsync(int allocationId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "dbo.sp_RoomAllocation_Vacate",
            new { AllocationId = allocationId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<AllocationOverviewResponse>> GetByPGIdAsync(int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AllocationOverviewResponse>(
            "dbo.sp_RoomAllocation_GetByPGId", new { PGId = pgId }, commandType: CommandType.StoredProcedure);
    }
}