using Microsoft.AspNetCore.Mvc;
using PGManagement.Application.DTOs;
using PGManagement.Application.Exceptions;
using PGManagement.Application.Interfaces;

namespace PGManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthController(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var id = await _userRepository.RegisterAsync(request.Email, passwordHash, request.FullName, request.Role);
            return Ok(new { id });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        });
    }
}