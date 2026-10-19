using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/results")]
[SessionGuard]
public class ResultsController(RaceDayContext db) : ControllerBase
{
    [HttpPost("enrolment/{enrolmentId:int}"), SessionGuard("Organiser")]
    public async Task<IActionResult> Record(int enrolmentId, ResultDto dto)
    {
        var enrolment = await db.Enrolments.Include(x => x.EventCategory).ThenInclude(x => x!.Event).SingleOrDefaultAsync(x => x.EnrolmentId == enrolmentId);
        if (enrolment is null) return NotFound();
        if (enrolment.EventCategory!.Event!.OrganiserId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (await db.Results.AnyAsync(x => x.EnrolmentId == enrolmentId)) return Conflict(new { message = "A result already exists for this enrolment." });
        TimeSpan? time = null;
        if (!string.IsNullOrWhiteSpace(dto.FinishTime))
        {
            if (!TimeSpan.TryParse(dto.FinishTime, out var parsed))
                return BadRequest(new { message = "FinishTime must be a TimeSpan such as 00:42:15." });
            time = parsed;
        }
        var result = new Result { EnrolmentId = enrolmentId, FinishPosition = dto.FinishPosition, FinishTime = time, ResultStatus = dto.ResultStatus };
        db.Results.Add(result); await db.SaveChangesAsync(); return Ok(result);
    }

    [HttpGet("mine"), SessionGuard("Participant")]
    public async Task<IActionResult> Mine()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await db.Results.Where(r => r.Enrolment!.ParticipantId == id)
            .Include(r => r.Enrolment).ThenInclude(e => e!.EventCategory).ThenInclude(ec => ec!.Event)
            .Select(r => new { r.ResultId, Event = r.Enrolment!.EventCategory!.Event!.Name, r.FinishPosition, r.FinishTime, r.ResultStatus, r.RecordedAt })
            .ToListAsync());
    }
}
