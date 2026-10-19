using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public int EventCategoryId { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(20)] public string Status { get; set; } = "Registered";
    public User? Participant { get; set; }
    public EventCategory? EventCategory { get; set; }
    public Result? Result { get; set; }
}
