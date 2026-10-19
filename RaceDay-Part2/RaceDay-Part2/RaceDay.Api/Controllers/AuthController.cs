using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RaceDayContext db) : ControllerBase
{
    private readonly PasswordHasher<User> _hasher = new();

    /// <summary>Register an organiser or participant. Passwords are stored as salted hashes.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var role = dto.Role.Trim();
        if (role is not ("Organiser" or "Participant"))
            return BadRequest(new { message = "Role must be Organiser or Participant." });
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { message = "An account with this email already exists." });
        var user = new User { FirstName = dto.FirstName.Trim(), LastName = dto.LastName.Trim(),
            Email = email, Role = role, PhoneNumber = dto.PhoneNumber?.Trim() };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Created($"/api/users/me", new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role });
    }

    /// <summary>Log in and establish a 30-minute server-side session.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid email or password." });
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("Email", user.Email);
        return Ok(new { message = "Login successful. Keep the session cookie for subsequent requests.", user.UserId, user.Email, user.Role });
    }

    [HttpPost("logout")]
    [SessionGuard]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logged out." });
    }

    [HttpGet("me")]
    [SessionGuard]
    public IActionResult Me() => Ok(new {
        UserId = HttpContext.Session.GetInt32("UserId"),
        Role = HttpContext.Session.GetString("Role"),
        Email = HttpContext.Session.GetString("Email")
    });
}
