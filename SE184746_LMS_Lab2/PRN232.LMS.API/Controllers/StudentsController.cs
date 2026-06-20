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

/// <summary>Student resources.</summary>
[ApiVersion("1.0")]
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

    /// <summary>Get a paged list of students with optional search, sort, field selection and expand.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeEnrollments = ExpandQueryHelper.IncludesEnrollments(expand);

        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size, includeEnrollments, ct);

        var fields = query.Fields?.Split(',').Select(f => f.Trim().ToLower()).ToHashSet();
        var responses = items.Select(s => ApplyFields(_mapper.Map<StudentResponse>(s), fields));

        return Ok(ApiResponse<PaginatedResponse<StudentResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a student by ID.</summary>
    [HttpGet("{id:int}", Name = "GetStudentById")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] int id, [FromQuery] string? expand, CancellationToken ct)
    {
        var parts = ExpandQueryHelper.Parse(expand);
        bool includeEnrollments = ExpandQueryHelper.IncludesEnrollments(parts);

        var student = await _service.GetByIdAsync(id, includeEnrollments, ct);
        if (student == null)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        return Ok(ApiResponse<StudentResponse>.Ok(_mapper.Map<StudentResponse>(student)));
    }

    /// <summary>Create a new student.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<StudentBusiness>(request), ct);
        var response = _mapper.Map<StudentResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.StudentId },
            ApiResponse<StudentResponse>.Ok(response, "Student created successfully."));
    }

    /// <summary>Update a student.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateStudentRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<StudentBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        var student = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<StudentResponse>.Ok(
            _mapper.Map<StudentResponse>(student!), "Student updated successfully."));
    }

    /// <summary>Delete a student.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Student {id} not found."));

        return NoContent();
    }

    private static StudentResponse ApplyFields(StudentResponse response, HashSet<string>? fields)
    {
        if (fields == null || fields.Count == 0) return response;
        return new StudentResponse
        {
            StudentId = fields.Contains("studentid") ? response.StudentId : null,
            FullName = fields.Contains("fullname") ? response.FullName : null,
            Email = fields.Contains("email") ? response.Email : null,
            DateOfBirth = fields.Contains("dateofbirth") ? response.DateOfBirth : null,
            StudentCode = fields.Contains("studentcode") ? response.StudentCode : null,
            PhoneNumber = fields.Contains("phonenumber") ? response.PhoneNumber : null,
            Enrollments = fields.Contains("enrollments") ? response.Enrollments : null
        };
    }
}
