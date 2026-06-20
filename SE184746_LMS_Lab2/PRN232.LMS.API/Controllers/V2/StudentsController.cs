using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.API.Controllers.V2;

/// <summary>
/// Student resources (v2) – StudentCode required, X-Request-Id tracing, Admin-only writes.
/// </summary>
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/students")]
[Authorize]
public sealed class StudentsController : LmsControllerBase
{
    private readonly IStudentService _service;
    private readonly IMapper _mapper;

    public StudentsController(IStudentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of students (v2).</summary>
    /// <remarks>Provide optional <c>X-Request-Id</c> header for request tracing.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentResponseV2>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryRequest query,
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(requestId))
            Response.Headers["X-Request-Id"] = requestId;

        var expand = ExpandQueryHelper.Parse(query.Expand);
        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size,
            ExpandQueryHelper.IncludesEnrollments(expand), ct);

        var responses = items.Select(s => _mapper.Map<StudentResponseV2>(s));
        return Ok(ApiResponse<PaginatedResponse<StudentResponseV2>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a student by ID (v2).</summary>
    [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseV2>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute][Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromQuery] string? expand,
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(requestId))
            Response.Headers["X-Request-Id"] = requestId;

        var parts = ExpandQueryHelper.Parse(expand);
        var student = await _service.GetByIdAsync(id, ExpandQueryHelper.IncludesEnrollments(parts), ct);

        if (student == null)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        return Ok(ApiResponse<StudentResponseV2>.Ok(_mapper.Map<StudentResponseV2>(student)));
    }

    /// <summary>Create a new student (v2, Admin only) – StudentCode is required.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseV2>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateStudentRequestV2 request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<StudentBusiness>(request), ct);
        var response = _mapper.Map<StudentResponseV2>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.StudentId },
            ApiResponse<StudentResponseV2>.Ok(response, "Student created successfully."));
    }

    /// <summary>Update a student (v2, Admin only) – StudentCode is required.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseV2>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        [FromRoute][Range(1, int.MaxValue)] int id,
        [FromBody] UpdateStudentRequestV2 request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<StudentBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        var student = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<StudentResponseV2>.Ok(
            _mapper.Map<StudentResponseV2>(student!), "Student updated successfully."));
    }

    /// <summary>Delete a student (v2, Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        [FromRoute][Range(1, int.MaxValue)] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        return NoContent();
    }
}
