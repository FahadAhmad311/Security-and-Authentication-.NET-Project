using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureApp.Data;
using SecureApp.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets user details (Admin access required).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found.");

        return Ok(user);
    }

    /// <summary>
    /// Updates user information (Admin access required).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found.");

        user.Email = updatedUser.Email;
        user.Password = updatedUser.Password; // Hash password before storing

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok("User updated successfully.");
    }
}
