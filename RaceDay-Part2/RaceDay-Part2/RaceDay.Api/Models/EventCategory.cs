using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class EventCategory
{
    public int EventCategoryId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public decimal EntryFee { get; set; }
    public int MaximumParticipants { get; set; }
    public Event? Event { get; set; }
    public Category? Category { get; set; }
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
