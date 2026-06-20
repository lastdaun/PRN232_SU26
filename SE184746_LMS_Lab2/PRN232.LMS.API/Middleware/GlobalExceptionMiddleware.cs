using System.Text.Json;
using System.Xml.Serialization;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Exceptions;

namespace PRN232.LMS.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ResourceNotFoundException ex)
        {
            await WriteErrorAsync(context, ex.Message, null, StatusCodes.Status404NotFound);
        }
        catch (ResourceValidationException ex)
        {
            await WriteErrorAsync(context, ex.Message, new[] { ex.Message }, StatusCodes.Status400BadRequest);
        }
        catch (InvalidCredentialsException ex)
        {
            await WriteErrorAsync(context, ex.Message, null, StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted) throw;

            var detail = _environment.IsDevelopment() ? ex.Message : null;
            await WriteErrorAsync(context, "An unexpected error occurred.", detail != null ? new[] { detail } : null,
                StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context, string message, string[]? errors, int statusCode)
    {
        if (context.Response.HasStarted) return;
        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        var acceptHeader = context.Request.Headers.Accept.ToString();
        var responseObj = new ErrorResponse { Success = false, Message = message, Errors = errors?.ToList() };

        if (acceptHeader.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.ContentType = "application/xml";
            var serializer = new XmlSerializer(typeof(ErrorResponse));
            using var sw = new StringWriter();
            serializer.Serialize(sw, responseObj);
            await context.Response.WriteAsync(sw.ToString());
        }
        else
        {
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(responseObj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            await context.Response.WriteAsync(json);
        }
    }
}

[XmlRoot("ApiResponse")]
public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;

    [XmlArray("Errors")]
    [XmlArrayItem("Error")]
    public List<string>? Errors { get; set; }
}
