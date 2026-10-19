using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
[SessionGuard]
public class EnrolmentsController(RaceDayContext db) : ControllerBase
{
    [HttpPost, SessionGuard("Participant")]
    public async Task<IActionResult> Enrol(EnrolmentDto dto)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var ec = await db.EventCategories.Include(x => x.Event).Include(x => x.Enrolments)
            .SingleOrDefaultAsync(x => x.EventCategoryId == dto.EventCategoryId);
        if (ec is null) return NotFound(new { message = "Event category not found." });
        if (DateTime.UtcNow > ec.Event!.RegistrationDeadline) return BadRequest(new { message = "Registration deadline has passed." });
        if (ec.Enrolments.Count >= ec.MaximumParticipants) return Conflict(new { message = "This category is full." });
        if (await db.Enrolments.AnyAsync(x => x.ParticipantId == userId && x.EventCategoryId == dto.EventCategoryId))
            return Conflict(new { message = "You are already enrolled in this category." });
        var enrolment = new Enrolment { ParticipantId = userId, EventCategoryId = dto.EventCategoryId };
        db.Enrolments.Add(enrolment); await db.SaveChangesAsync();
        return Created($"/api/enrolments/{enrolment.EnrolmentId}", new { enrolment.EnrolmentId, enrolment.ParticipantId, enrolment.EventCategoryId, enrolment.EnrolmentDate, enrolment.Status });
    }

    [HttpGet("mine"), SessionGuard("Participant")]
    public async Task<IActionResult> Mine()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await db.Enrolments.Where(x => x.ParticipantId == id).Include(x => x.EventCategory).ThenInclude(x => x!.Event)
            .Select(x => new { x.EnrolmentId, Event = x.EventCategory!.Event!.Name, x.EventCategory.EventCategoryId, x.EnrolmentDate, x.Status })
            .ToListAsync());
    }

    [HttpGet("event/{eventId:int}"), SessionGuard("Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;
        var ev = await db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();
        return Ok(await db.Enrolments.Where(x => x.EventCategory!.EventId == eventId)
            .Include(x => x.Participant).Include(x => x.EventCategory).ThenInclude(x => x!.Category)
            .Select(x => new { x.EnrolmentId, Participant = x.Participant!.FirstName + " " + x.Participant.LastName,
                x.Participant.Email, Category = x.EventCategory!.Category!.Name, x.EnrolmentDate, x.Status }).ToListAsync());
    }
}
