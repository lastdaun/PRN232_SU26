using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Responses;

namespace PRN232.LMS.API.Extensions;

public static class ApiValidationExtensions
{
    public static IServiceCollection AddLmsApiValidation(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Invalid value."
                                : error.ErrorMessage)
                            .ToArray());

                var response = ApiResponse<object>.Fail("Validation failed.", errors);
                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }
}
