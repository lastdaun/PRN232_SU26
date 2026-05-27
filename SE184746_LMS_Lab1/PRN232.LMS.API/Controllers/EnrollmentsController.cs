using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

/// <summary>Enrollment resources.</summary>
[Route("api/enrollments")]
public sealed class EnrollmentsController : LmsControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>Get a paged list of enrollments.</summary>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetEnrollments")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments(
        [FromQuery] GetEnrollmentsRequest request,
        CancellationToken cancellationToken = default)
        => OkList(await _enrollmentService.GetEnrollmentsAsync(request, cancellationToken));

    /// <summary>Get an enrollment by ID.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(OperationId = "GetEnrollmentById")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery][MaxLength(500)] string? expand,
        CancellationToken cancellationToken = default)
        => FromGetById(await _enrollmentService.GetEnrollmentByIdAsync(
            ToGetByIdRequest<GetEnrollmentByIdRequest>(id, expand),
            cancellationToken));

    /// <summary>Create a new enrollment.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateEnrollment")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _enrollmentService.CreateEnrollmentAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return FromCreate(
            response,
            nameof(GetEnrollmentById),
            ((EnrollmentResponse)response.Data!).EnrollmentId);
    }

    /// <summary>Update an enrollment.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(OperationId = "UpdateEnrollment")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollment(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken cancellationToken = default)
        => FromUpdate(await _enrollmentService.UpdateEnrollmentAsync(id, request, cancellationToken));

    /// <summary>Delete an enrollment.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "DeleteEnrollment")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken = default)
        => FromDelete(await _enrollmentService.DeleteEnrollmentAsync(id, cancellationToken));
}
