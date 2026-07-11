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

    //public async Task<int> CreateAsync(int tenantId, CreateBookingRequest request)
    //{
    //    using var connection = _connectionFactory.CreateConnection();
    //    return await connection.ExecuteScalarAsync<int>(
    // "dbo.sp_Booking_Insert",
    // new
    // {
    //     TenantId = tenantId,
    //     request.RoomId,
    //     request.Message
    // },
    // commandType: CommandType.StoredProcedure);
    //}

    public async Task<int> CreateAsync(string userId, CreateBookingRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        // 1. Check if room exists first
        var roomExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(1) FROM dbo.Room WHERE Id = @RoomId",
            new { request.RoomId }
        );

        if (!roomExists)
        {
            return -1; // Or throw a custom Exception like: throw new KeyNotFoundException("Room not found");
        }

        // 2. Proceed with insert if it does
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Booking_Insert",
            new
            {
                UserId = userId,
                request.RoomId,
                request.Message
            },
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

    public async Task ApproveBookingAsync(int bookingId)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.sp_Booking_Approve",
            new { BookingId = bookingId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<BookingResponse>> GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<BookingResponse>(
            "dbo.sp_Booking_GetByUserId",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }
}