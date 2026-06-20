using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.API.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class FptuStudentCodeAttribute : ValidationAttribute
{
    private static readonly Regex Pattern = new(@"^[A-Z]{2}\d{5,6}$", RegexOptions.Compiled);

    public FptuStudentCodeAttribute()
        : base("The field {0} must be a valid FPTU student code (e.g. SE184746, CE12345).")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var code = value as string;
        if (code is null || !Pattern.IsMatch(code))
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

        return ValidationResult.Success;
    }
}
