using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

/// <summary>Course resources.</summary>
[Route("api/courses")]
public sealed class CoursesController : LmsControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>Get a paged list of courses.</summary>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetCourses")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] GetCoursesRequest request,
        CancellationToken cancellationToken = default)
        => OkList(await _courseService.GetCoursesAsync(request, cancellationToken));

    /// <summary>Get a course by ID.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(OperationId = "GetCourseById")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery][MaxLength(500)] string? expand,
        CancellationToken cancellationToken = default)
        => FromGetById(await _courseService.GetCourseByIdAsync(
            ToGetByIdRequest<GetCourseByIdRequest>(id, expand),
            cancellationToken));

    /// <summary>Get enrollments for a course.</summary>
    /// <remarks>
    /// Example: GET /api/courses/1/enrollments?page=1&amp;size=10&amp;sort=-enrollDate&amp;search=active&amp;fields=enrollmentId,status&amp;expand=student
    /// </remarks>
    [HttpGet("{id:int}/enrollments")]
    [SwaggerOperation(OperationId = "GetCourseEnrollments")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseEnrollments(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery] GetCourseEnrollmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _courseService.GetCourseEnrollmentsAsync(id, request, cancellationToken);
        return response.Success ? OkList(response) : NotFound(response);
    }

    /// <summary>Create a new course.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateCourse")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourse(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _courseService.CreateCourseAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return FromCreate(
            response,
            nameof(GetCourseById),
            ((CourseResponse)response.Data!).CourseId);
    }

    /// <summary>Update a course.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(OperationId = "UpdateCourse")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken = default)
        => FromUpdate(await _courseService.UpdateCourseAsync(id, request, cancellationToken));

    /// <summary>Delete a course.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "DeleteCourse")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken = default)
        => FromDelete(await _courseService.DeleteCourseAsync(id, cancellationToken));
}
