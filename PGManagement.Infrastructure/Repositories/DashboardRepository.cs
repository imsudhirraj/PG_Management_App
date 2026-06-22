using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public DashboardRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<OwnerDashboardStats> GetOwnerStatsAsync(string ownerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stats = await connection.QueryFirstOrDefaultAsync<OwnerDashboardStats>(
            "dbo.sp_Owner_DashboardStats", new { OwnerId = ownerId }, commandType: CommandType.StoredProcedure);
        return stats ?? new OwnerDashboardStats();
    }
}