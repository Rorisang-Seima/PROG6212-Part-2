using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Dtos;

public record RegisterDto([Required, MaxLength(80)] string FirstName,
    [Required, MaxLength(80)] string LastName,
    [Required, EmailAddress, MaxLength(180)] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string Role,
    string? PhoneNumber);

public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);
public record ProfileUpdateDto([Required, MaxLength(80)] string FirstName,
    [Required, MaxLength(80)] string LastName, string? PhoneNumber);
public record EventDto(string Name, string? Description, DateTime EventDate, string Location,
    DateTime RegistrationDeadline, decimal DistanceKm, string EventType, string? ImageUrl, string? RouteInfoUrl);
public record EventCategoryDto(int CategoryId, decimal EntryFee, int MaximumParticipants);
public record EnrolmentDto(int EventCategoryId);
public record ResultDto(int? FinishPosition, string? FinishTime, string ResultStatus);
