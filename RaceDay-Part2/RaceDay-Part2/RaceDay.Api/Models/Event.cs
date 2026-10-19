using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Event
{
    public int EventId { get; set; }
    public int OrganiserId { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [MaxLength(2000)] public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    [Required, MaxLength(250)] public string Location { get; set; } = "";
    public DateTime RegistrationDeadline { get; set; }
    [MaxLength(500)] public string? ImageUrl { get; set; }
    [MaxLength(500)] public string? RouteInfoUrl { get; set; }
    [Range(0.1, 1000)] public decimal DistanceKm { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Run";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? Organiser { get; set; }
    public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
}
