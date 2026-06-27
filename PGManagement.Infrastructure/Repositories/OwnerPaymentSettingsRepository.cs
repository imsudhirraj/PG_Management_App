using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class OwnerPaymentSettingsRepository
    : IOwnerPaymentSettingsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OwnerPaymentSettingsRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SaveAsync(
        string ownerId,
        OwnerPaymentSettingsRequest request)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.sp_OwnerPaymentSettings_Save",
            new
            {
                OwnerId = ownerId,
                request.AccountHolderName,
                request.UpiId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<OwnerPaymentSettingsResponse?> GetAsync(
        string ownerId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<OwnerPaymentSettingsResponse>(
            "dbo.sp_OwnerPaymentSettings_Get",
            new { OwnerId = ownerId },
            commandType: CommandType.StoredProcedure);
    }
}