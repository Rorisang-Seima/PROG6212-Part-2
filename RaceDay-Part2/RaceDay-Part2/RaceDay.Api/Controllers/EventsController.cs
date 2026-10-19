using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEvents() => Ok(await db.Events
        .Include(e => e.Organiser).Select(e => new { e.EventId, e.Name, e.Description, e.EventDate, e.Location,
            e.RegistrationDeadline, e.DistanceKm, e.EventType, e.ImageUrl, e.RouteInfoUrl,
            OrganiserName = e.Organiser!.FirstName + " " + e.Organiser.LastName })
        .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var ev = await db.Events.Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
            .Where(e => e.EventId == id).Select(e => new { e.EventId, e.Name, e.Description, e.EventDate,
                e.Location, e.RegistrationDeadline, e.DistanceKm, e.EventType, Categories = e.EventCategories.Select(ec =>
                new { ec.EventCategoryId, ec.EntryFee, ec.MaximumParticipants, ec.Category!.Name, ec.CategoryId }) }).SingleOrDefaultAsync();
        return ev is null ? NotFound() : Ok(ev);
    }

    [HttpPost, SessionGuard("Organiser")]
    public async Task<IActionResult> CreateEvent(EventDto dto)
    {
        if (!ValidType(dto.EventType)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        var ev = new Event { OrganiserId = HttpContext.Session.GetInt32("UserId")!.Value,
            Name = dto.Name.Trim(), Description = dto.Description, EventDate = dto.EventDate, Location = dto.Location.Trim(),
            RegistrationDeadline = dto.RegistrationDeadline, DistanceKm = dto.DistanceKm, EventType = dto.EventType,
            ImageUrl = dto.ImageUrl, RouteInfoUrl = dto.RouteInfoUrl };
        db.Events.Add(ev); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = ev.EventId }, ev);
    }

    [HttpPut("{id:int}"), SessionGuard("Organiser")]
    public async Task<IActionResult> UpdateEvent(int id, EventDto dto)
    {
        var ev = await db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (!ValidType(dto.EventType)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        ev.Name = dto.Name.Trim(); ev.Description = dto.Description; ev.EventDate = dto.EventDate; ev.Location = dto.Location.Trim();
        ev.RegistrationDeadline = dto.RegistrationDeadline; ev.DistanceKm = dto.DistanceKm; ev.EventType = dto.EventType;
        ev.ImageUrl = dto.ImageUrl; ev.RouteInfoUrl = dto.RouteInfoUrl;
        await db.SaveChangesAsync(); return Ok(ev);
    }

    [HttpDelete("{id:int}"), SessionGuard("Organiser")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        db.Events.Remove(ev); await db.SaveChangesAsync(); return NoContent();
    }

    [HttpPost("{id:int}/categories"), SessionGuard("Organiser")]
    public async Task<IActionResult> AddCategory(int id, EventCategoryDto dto)
    {
        var ev = await db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (!await db.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId)) return BadRequest(new { message = "Category does not exist." });
        if (await db.EventCategories.AnyAsync(x => x.EventId == id && x.CategoryId == dto.CategoryId))
            return Conflict(new { message = "This category is already assigned to the event." });
        var ec = new EventCategory { EventId = id, CategoryId = dto.CategoryId, EntryFee = dto.EntryFee, MaximumParticipants = dto.MaximumParticipants };
        db.EventCategories.Add(ec); await db.SaveChangesAsync(); return Ok(ec);
    }

    private static bool ValidType(string value) => new[] { "Run", "Walk", "Cycle" }.Contains(value, StringComparer.OrdinalIgnoreCase);
}
