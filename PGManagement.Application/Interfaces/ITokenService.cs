using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserRecord user);
}