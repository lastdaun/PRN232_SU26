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

/// <summary>Semester resources.</summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/semesters")]
[Authorize]
public sealed class SemestersController : LmsControllerBase
{
    private readonly ISemesterService _service;
    private readonly IMapper _mapper;

    public SemestersController(ISemesterService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of semesters.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SemesterResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeCourses = ExpandQueryHelper.Includes(expand, "courses");

        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size, includeCourses, ct);

        var fields = query.Fields?.Split(',').Select(f => f.Trim().ToLower()).ToHashSet();
        var responses = items.Select(s => ApplyFields(_mapper.Map<SemesterResponse>(s), fields));

        return Ok(ApiResponse<PaginatedResponse<SemesterResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a semester by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] int id, [FromQuery] string? expand, CancellationToken ct)
    {
        var parts = ExpandQueryHelper.Parse(expand);
        var semester = await _service.GetByIdAsync(id, ExpandQueryHelper.Includes(parts, "courses"), ct);

        if (semester == null)
            return NotFound(ApiResponse<object>.Fail($"Semester {id} not found."));

        return Ok(ApiResponse<SemesterResponse>.Ok(_mapper.Map<SemesterResponse>(semester)));
    }

    /// <summary>Create a new semester.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<SemesterBusiness>(request), ct);
        var response = _mapper.Map<SemesterResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.SemesterId },
            ApiResponse<SemesterResponse>.Ok(response, "Semester created successfully."));
    }

    /// <summary>Update a semester.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateSemesterRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<SemesterBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Semester {id} not found."));

        var semester = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<SemesterResponse>.Ok(
            _mapper.Map<SemesterResponse>(semester!), "Semester updated successfully."));
    }

    /// <summary>Delete a semester.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Semester {id} not found."));

        return NoContent();
    }

    private static SemesterResponse ApplyFields(SemesterResponse response, HashSet<string>? fields)
    {
        if (fields == null || fields.Count == 0) return response;
        return new SemesterResponse
        {
            SemesterId = fields.Contains("semesterid") ? response.SemesterId : null,
            SemesterName = fields.Contains("semestername") ? response.SemesterName : null,
            StartDate = fields.Contains("startdate") ? response.StartDate : null,
            EndDate = fields.Contains("enddate") ? response.EndDate : null,
            Courses = fields.Contains("courses") ? response.Courses : null
        };
    }
}
