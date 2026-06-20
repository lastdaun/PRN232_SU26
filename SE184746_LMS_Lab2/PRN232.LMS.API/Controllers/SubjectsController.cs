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

/// <summary>Subject resources.</summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subjects")]
[Authorize]
public sealed class SubjectsController : LmsControllerBase
{
    private readonly ISubjectService _service;
    private readonly IMapper _mapper;

    public SubjectsController(ISubjectService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of subjects.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SubjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeCourses = ExpandQueryHelper.Includes(expand, "courses");

        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size, includeCourses, ct);

        var fields = query.Fields?.Split(',').Select(f => f.Trim().ToLower()).ToHashSet();
        var responses = items.Select(s => ApplyFields(_mapper.Map<SubjectResponse>(s), fields));

        return Ok(ApiResponse<PaginatedResponse<SubjectResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a subject by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] int id, [FromQuery] string? expand, CancellationToken ct)
    {
        var parts = ExpandQueryHelper.Parse(expand);
        var subject = await _service.GetByIdAsync(id, ExpandQueryHelper.Includes(parts, "courses"), ct);

        if (subject == null)
            return NotFound(ApiResponse<object>.Fail($"Subject {id} not found."));

        return Ok(ApiResponse<SubjectResponse>.Ok(_mapper.Map<SubjectResponse>(subject)));
    }

    /// <summary>Create a new subject.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<SubjectBusiness>(request), ct);
        var response = _mapper.Map<SubjectResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.SubjectId },
            ApiResponse<SubjectResponse>.Ok(response, "Subject created successfully."));
    }

    /// <summary>Update a subject.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateSubjectRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<SubjectBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Subject {id} not found."));

        var subject = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<SubjectResponse>.Ok(
            _mapper.Map<SubjectResponse>(subject!), "Subject updated successfully."));
    }

    /// <summary>Delete a subject.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Subject {id} not found."));

        return NoContent();
    }

    private static SubjectResponse ApplyFields(SubjectResponse response, HashSet<string>? fields)
    {
        if (fields == null || fields.Count == 0) return response;
        return new SubjectResponse
        {
            SubjectId = fields.Contains("subjectid") ? response.SubjectId : null,
            SubjectCode = fields.Contains("subjectcode") ? response.SubjectCode : null,
            SubjectName = fields.Contains("subjectname") ? response.SubjectName : null,
            Credit = fields.Contains("credit") ? response.Credit : null,
            Courses = fields.Contains("courses") ? response.Courses : null
        };
    }
}
