using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

/// <summary>Semester resources.</summary>
[Route("api/semesters")]
public sealed class SemestersController : LmsControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    /// <summary>Get a paged list of semesters.</summary>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetSemesters")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesters(
        [FromQuery] GetSemestersRequest request,
        CancellationToken cancellationToken = default)
        => OkList(await _semesterService.GetSemestersAsync(request, cancellationToken));

    /// <summary>Get a semester by ID.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(OperationId = "GetSemesterById")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSemesterById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery][MaxLength(500)] string? expand,
        CancellationToken cancellationToken = default)
        => FromGetById(await _semesterService.GetSemesterByIdAsync(
            ToGetByIdRequest<GetSemesterByIdRequest>(id, expand),
            cancellationToken));

    /// <summary>Create a new semester.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateSemester")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSemester(
        [FromBody] CreateSemesterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _semesterService.CreateSemesterAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return FromCreate(
            response,
            nameof(GetSemesterById),
            ((SemesterResponse)response.Data!).SemesterId);
    }

    /// <summary>Update a semester.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(OperationId = "UpdateSemester")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSemester(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateSemesterRequest request,
        CancellationToken cancellationToken = default)
        => FromUpdate(await _semesterService.UpdateSemesterAsync(id, request, cancellationToken));

    /// <summary>Delete a semester.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "DeleteSemester")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSemester(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken = default)
        => FromDelete(await _semesterService.DeleteSemesterAsync(id, cancellationToken));
}
