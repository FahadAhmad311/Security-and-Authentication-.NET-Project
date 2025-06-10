using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureApp.Data;
using SecureApp.Models;
using SecureApp.Services;
using System.Linq;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public AuthController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    /// <summary>
    /// User Registration.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if (_context.Users.Any(u => u.Email == user.Email))
            return BadRequest("Email already exists.");

        user.Password = _authService.HashPassword(user.Password); // Secure password

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    /// <summary>
    /// User Login with JWT authentication.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !_authService.ValidatePassword(request.Password, user.Password))
            return Unauthorized("Invalid credentials.");

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
}

/// <summary>
/// Represents user login request model.
/// </summary>
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
