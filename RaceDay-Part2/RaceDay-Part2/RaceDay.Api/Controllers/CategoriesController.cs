using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(RaceDayContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Categories.OrderBy(c => c.Name).ToListAsync());

    [HttpPost, SessionGuard("Organiser")]
    public async Task<IActionResult> Create(Category dto)
    {
        dto.CategoryId = 0;
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { message = "Category name is required." });
        if (await db.Categories.AnyAsync(c => c.Name == dto.Name.Trim())) return Conflict(new { message = "Category name already exists." });
        dto.Name = dto.Name.Trim(); db.Categories.Add(dto); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = dto.CategoryId }, dto);
    }
}
