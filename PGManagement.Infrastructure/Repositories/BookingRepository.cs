using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public BookingRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(int tenantId, CreateBookingRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Booking_Insert",
            new { TenantId = tenantId, request.RoomId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<BookingResponse>> GetByTenantIdAsync(int tenantId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<BookingResponse>(
            "dbo.sp_Booking_GetByTenantId",
            new { TenantId = tenantId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<BookingResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<BookingResponse>(
            "dbo.sp_Booking_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "dbo.sp_Booking_UpdateStatus",
            new { Id = id, Status = status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<BookingOverviewResponse>> GetByOwnerIdAsync(string ownerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<BookingOverviewResponse>(
            "dbo.sp_Booking_GetByOwnerId", new { OwnerId = ownerId }, commandType: CommandType.StoredProcedure);
    }
}