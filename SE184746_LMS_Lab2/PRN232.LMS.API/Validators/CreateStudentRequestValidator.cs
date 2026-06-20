using FluentValidation;
using PRN232.LMS.API.Models.Requests;

namespace PRN232.LMS.API.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
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
            .Matches(@"^[A-Z]{2}\d{5,6}$")
            .WithMessage("StudentCode must be a valid FPTU student code (e.g. SE184746).")
            .When(x => !string.IsNullOrEmpty(x.StudentCode));
    }
}
