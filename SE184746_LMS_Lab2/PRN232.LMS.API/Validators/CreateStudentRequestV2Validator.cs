using FluentValidation;
using PRN232.LMS.API.Models.Requests;
using System.Text.RegularExpressions;

namespace PRN232.LMS.API.Validators;

public class CreateStudentRequestV2Validator : AbstractValidator<CreateStudentRequestV2>
{
    private static readonly Regex StudentCodeRegex = new(
        @"^[A-Z]{2}\d{5,6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public CreateStudentRequestV2Validator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .MaximumLength(100).WithMessage("FullName cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirth is required.")
            .LessThan(DateTime.Today).WithMessage("DateOfBirth must be in the past.");

        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("StudentCode is required in v2.")
            .Must(code => code != null && StudentCodeRegex.IsMatch(code))
            .WithMessage("StudentCode must be a valid FPTU student code (e.g. SE184746).");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("PhoneNumber cannot exceed 20 characters.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
