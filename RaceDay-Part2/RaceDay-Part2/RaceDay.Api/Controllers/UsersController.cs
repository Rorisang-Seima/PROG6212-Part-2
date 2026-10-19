using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/users")]
[SessionGuard]
public class UsersController(RaceDayContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var id = HttpContext.Session.GetInt32("UserId");
        var user = await db.Users.Where(x => x.UserId == id)
            .Select(x => new { x.UserId, x.FirstName, x.LastName, x.Email, x.Role, x.PhoneNumber, x.CreatedAt })
            .SingleOrDefaultAsync();
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(ProfileUpdateDto dto)
    {
        var id = HttpContext.Session.GetInt32("UserId");
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.PhoneNumber = dto.PhoneNumber?.Trim();
        await db.SaveChangesAsync();
        return Ok(new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.PhoneNumber });
    }
}
