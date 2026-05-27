using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

/// <summary>Student resources.</summary>
[Route("api/students")]
public sealed class StudentsController : LmsControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>Get a paged list of students.</summary>
    /// <remarks>
    /// Example: GET /api/students?search=nguyen&amp;sort=fullName,-dateOfBirth&amp;page=1&amp;size=10&amp;fields=studentId,fullName,email&amp;expand=enrollments
    /// </remarks>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetStudents")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents(
        [FromQuery] GetStudentsRequest request,
        CancellationToken cancellationToken = default)
        => OkList(await _studentService.GetStudentsAsync(request, cancellationToken));

    /// <summary>Get a student by ID.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(OperationId = "GetStudentById")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery][MaxLength(500)] string? expand,
        CancellationToken cancellationToken = default)
        => FromGetById(await _studentService.GetStudentByIdAsync(
            ToGetByIdRequest<GetStudentByIdRequest>(id, expand),
            cancellationToken));

    /// <summary>Create a new student.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateStudent")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStudent(
        [FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _studentService.CreateStudentAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return FromCreate(
            response,
            nameof(GetStudentById),
            ((StudentResponse)response.Data!).StudentId);
    }

    /// <summary>Update a student.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(OperationId = "UpdateStudent")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken = default)
        => FromUpdate(await _studentService.UpdateStudentAsync(id, request, cancellationToken));

    /// <summary>Delete a student.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "DeleteStudent")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken = default)
        => FromDelete(await _studentService.DeleteStudentAsync(id, cancellationToken));
}
