using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.API.Controllers;

/// <summary>Enrollment resources.</summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/enrollments")]
[Authorize]
public sealed class EnrollmentsController : LmsControllerBase
{
    private readonly IEnrollmentService _service;
    private readonly IMapper _mapper;

    public EnrollmentsController(IEnrollmentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of enrollments.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EnrollmentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeStudent = ExpandQueryHelper.Includes(expand, "student");
        bool includeCourse = ExpandQueryHelper.Includes(expand, "course");

        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size, includeStudent, includeCourse, ct);

        var responses = items.Select(e =>
        {
            var r = _mapper.Map<EnrollmentResponse>(e);
            if (r.Student != null) r.Student.Enrollments = null;
            if (r.Course != null) r.Course.Enrollments = null;
            return r;
        });

        return Ok(ApiResponse<PaginatedResponse<EnrollmentResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get an enrollment by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] int id, [FromQuery] string? expand, CancellationToken ct)
    {
        var parts = ExpandQueryHelper.Parse(expand);
        var enrollment = await _service.GetByIdAsync(id,
            ExpandQueryHelper.Includes(parts, "student"),
            ExpandQueryHelper.Includes(parts, "course"), ct);

        if (enrollment == null)
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} not found."));

        var response = _mapper.Map<EnrollmentResponse>(enrollment);
        if (response.Student != null) response.Student.Enrollments = null;
        if (response.Course != null) response.Course.Enrollments = null;

        return Ok(ApiResponse<EnrollmentResponse>.Ok(response));
    }

    /// <summary>Create a new enrollment.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<EnrollmentBusiness>(request), ct);
        var response = _mapper.Map<EnrollmentResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.EnrollmentId },
            ApiResponse<EnrollmentResponse>.Ok(response, "Enrollment created successfully."));
    }

    /// <summary>Update an enrollment.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateEnrollmentRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<EnrollmentBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} not found."));

        var enrollment = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(
            _mapper.Map<EnrollmentResponse>(enrollment!), "Enrollment updated successfully."));
    }

    /// <summary>Patch enrollment status.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchStatus(
        [FromRoute] int id, [FromBody] PatchEnrollmentStatusRequest request, CancellationToken ct)
    {
        var enrollment = await _service.GetByIdAsync(id, cancellationToken: ct);
        if (enrollment == null)
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} not found."));

        enrollment.Status = request.Status;
        await _service.UpdateAsync(id, enrollment, ct);

        var updated = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(
            _mapper.Map<EnrollmentResponse>(updated!), "Enrollment status updated successfully."));
    }

    /// <summary>Delete an enrollment.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} not found."));

        return NoContent();
    }
}
