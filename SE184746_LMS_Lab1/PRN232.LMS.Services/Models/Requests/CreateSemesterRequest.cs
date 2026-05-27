using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

/// <summary>Request body for creating a semester.</summary>
public sealed class CreateSemesterRequest : IValidatableObject
{
    [Required(ErrorMessage = "SemesterName is required.")]
    [MaxLength(100, ErrorMessage = "SemesterName must not exceed 100 characters.")]
    public string SemesterName { get; init; } = string.Empty;

    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime StartDate { get; init; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime EndDate { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "EndDate must be after StartDate.",
                [nameof(EndDate), nameof(StartDate)]);
        }
    }
}
