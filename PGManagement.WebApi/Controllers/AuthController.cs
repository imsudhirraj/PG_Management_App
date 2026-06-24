using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    //[HttpGet("db-test")]
    //public IActionResult DbConn()
    //{
    //    return Ok(_configuration.GetConnectionString("DefaultConnection"));
    //}

    //[HttpGet("db-test")]
    //public async Task<IActionResult> DbTest()
    //{
    //    try
    //    {
    //        using var conn = new SqlConnection(
    //            _configuration.GetConnectionString("DefaultConnection"));

    //        await conn.OpenAsync();

    //        return Ok("Database connected successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Ok(ex.ToString());
    //    }
    //}

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

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginRequest request)
    //{
    //    var user = await _userRepository.GetByEmailAsync(request.Email);
    //    if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    //        return Unauthorized(new { message = "Invalid email or password." });

    //    var token = _tokenService.GenerateToken(user);
    //    return Ok(new AuthResponse
    //    {
    //        Token = token,
    //        UserId = user.Id,
    //        Email = user.Email,
    //        FullName = user.FullName,
    //        Role = user.Role
    //    });
    //}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                return Unauthorized(new { message = "User not found" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid password" });

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
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = ex.Message,
                InnerError = ex.InnerException?.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}