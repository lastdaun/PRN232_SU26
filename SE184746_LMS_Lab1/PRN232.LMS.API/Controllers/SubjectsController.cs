using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.LMS.API.Controllers;

/// <summary>Subject resources.</summary>
[Route("api/subjects")]
public sealed class SubjectsController : LmsControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    /// <summary>Get a paged list of subjects.</summary>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetSubjects")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjects(
        [FromQuery] GetSubjectsRequest request,
        CancellationToken cancellationToken = default)
        => OkList(await _subjectService.GetSubjectsAsync(request, cancellationToken));

    /// <summary>Get a subject by ID.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(OperationId = "GetSubjectById")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery][MaxLength(500)] string? expand,
        CancellationToken cancellationToken = default)
        => FromGetById(await _subjectService.GetSubjectByIdAsync(
            ToGetByIdRequest<GetSubjectByIdRequest>(id, expand),
            cancellationToken));

    /// <summary>Create a new subject.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateSubject")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSubject(
        [FromBody] CreateSubjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _subjectService.CreateSubjectAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return FromCreate(
            response,
            nameof(GetSubjectById),
            ((SubjectResponse)response.Data!).SubjectId);
    }

    /// <summary>Update a subject.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(OperationId = "UpdateSubject")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubject(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateSubjectRequest request,
        CancellationToken cancellationToken = default)
        => FromUpdate(await _subjectService.UpdateSubjectAsync(id, request, cancellationToken));

    /// <summary>Delete a subject.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(OperationId = "DeleteSubject")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken = default)
        => FromDelete(await _subjectService.DeleteSubjectAsync(id, cancellationToken));
}
