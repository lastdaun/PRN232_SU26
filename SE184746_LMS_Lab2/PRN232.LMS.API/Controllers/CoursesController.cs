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

/// <summary>Course resources.</summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
[Authorize]
public sealed class CoursesController : LmsControllerBase
{
    private readonly ICourseService _service;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of courses.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CourseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeSemester = ExpandQueryHelper.Includes(expand, "semester");
        bool includeSubject = ExpandQueryHelper.Includes(expand, "subject");
        bool includeEnrollments = ExpandQueryHelper.IncludesEnrollments(expand);

        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size,
            includeSemester, includeSubject, includeEnrollments, ct);

        var fields = query.Fields?.Split(',').Select(f => f.Trim().ToLower()).ToHashSet();
        var responses = items.Select(c => ApplyFields(_mapper.Map<CourseResponse>(c), fields));

        return Ok(ApiResponse<PaginatedResponse<CourseResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a course by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] int id, [FromQuery] string? expand, CancellationToken ct)
    {
        var parts = ExpandQueryHelper.Parse(expand);
        var course = await _service.GetByIdAsync(id,
            ExpandQueryHelper.Includes(parts, "semester"),
            ExpandQueryHelper.Includes(parts, "subject"),
            ExpandQueryHelper.IncludesEnrollments(parts), ct);

        if (course == null)
            return NotFound(ApiResponse<object>.Fail($"Course {id} not found."));

        return Ok(ApiResponse<CourseResponse>.Ok(_mapper.Map<CourseResponse>(course)));
    }

    /// <summary>Get enrollments for a specific course.</summary>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EnrollmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseEnrollments(
        [FromRoute] int id, [FromQuery] CourseEnrollmentsQueryRequest query, CancellationToken ct)
    {
        var expand = ExpandQueryHelper.Parse(query.Expand);
        bool includeStudent = ExpandQueryHelper.Includes(expand, "student");
        bool includeCourse = ExpandQueryHelper.Includes(expand, "course");

        var (items, total) = await _service.GetCourseEnrollmentsAsync(
            id, query.Search, query.Sort, query.Page, query.Size, includeStudent, includeCourse, ct);

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

    /// <summary>Get students enrolled in a specific course (nested resource).</summary>
    [HttpGet("{courseId:int}/students")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseStudents(
        [FromRoute] int courseId, [FromQuery] CourseEnrollmentsQueryRequest query, CancellationToken ct)
    {
        var (enrollments, total) = await _service.GetCourseEnrollmentsAsync(
            courseId, query.Search, query.Sort, query.Page, query.Size,
            includeStudent: true, includeCourse: false, ct);

        var students = enrollments
            .Where(e => e.Student != null)
            .Select(e => _mapper.Map<StudentResponse>(e.Student!));

        return Ok(ApiResponse<PaginatedResponse<StudentResponse>>.Ok(
            ToPaginatedResponse(students, total, query.Page, query.Size)));
    }

    /// <summary>Create a new course.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<CourseBusiness>(request), ct);
        var response = _mapper.Map<CourseResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.CourseId },
            ApiResponse<CourseResponse>.Ok(response, "Course created successfully."));
    }

    /// <summary>Update a course.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateCourseRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<CourseBusiness>(request), ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Course {id} not found."));

        var course = await _service.GetByIdAsync(id, cancellationToken: ct);
        return Ok(ApiResponse<CourseResponse>.Ok(
            _mapper.Map<CourseResponse>(course!), "Course updated successfully."));
    }

    /// <summary>Delete a course.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Course {id} not found."));

        return NoContent();
    }

    private static CourseResponse ApplyFields(CourseResponse response, HashSet<string>? fields)
    {
        if (fields == null || fields.Count == 0) return response;
        return new CourseResponse
        {
            CourseId = fields.Contains("courseid") ? response.CourseId : null,
            CourseName = fields.Contains("coursename") ? response.CourseName : null,
            SemesterId = fields.Contains("semesterid") ? response.SemesterId : null,
            SubjectId = fields.Contains("subjectid") ? response.SubjectId : null,
            Semester = fields.Contains("semester") ? response.Semester : null,
            Subject = fields.Contains("subject") ? response.Subject : null,
            Enrollments = fields.Contains("enrollments") ? response.Enrollments : null
        };
    }
}
