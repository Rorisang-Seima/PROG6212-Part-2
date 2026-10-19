using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Result
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public int? FinishPosition { get; set; }
    public TimeSpan? FinishTime { get; set; }
    [Required, MaxLength(30)] public string ResultStatus { get; set; } = "Finished";
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public Enrolment? Enrolment { get; set; }
}
