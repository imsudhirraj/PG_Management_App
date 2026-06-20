using Dapper;
using Microsoft.Data.SqlClient;
using PGManagement.Application.DTOs;
using PGManagement.Application.Exceptions;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public UserRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> RegisterAsync(string email, string passwordHash, string fullName, string role)
    {
        using var connection = _connectionFactory.CreateConnection();
        try
        {
            return await connection.ExecuteScalarAsync<int>(
                "dbo.sp_User_Register",
                new { Email = email, PasswordHash = passwordHash, FullName = fullName, Role = role },
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Message.Contains("already registered"))
        {
            throw new BusinessRuleException(ex.Message);
        }
    }

    public async Task<UserRecord?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserRecord>(
            "dbo.sp_User_GetByEmail",
            new { Email = email },
            commandType: CommandType.StoredProcedure);
    }
}