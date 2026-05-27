using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Sorting;

namespace PRN232.LMS.Services.Implementations;

public sealed class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly ISubjectRepository _subjectRepository;

    public CourseService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ISemesterRepository semesterRepository,
        ISubjectRepository subjectRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _semesterRepository = semesterRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<ApiResponse<object>> GetCoursesAsync(GetCoursesRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PagedListResultBuilder.CreateAsync(
            request,
            _courseRepository.QueryCourses(request.GetExpandTokens()),
            query => ApplySearch(query, request.Search),
            CourseSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToCourseResponse(
                EntityToBusinessMapper.ToCourseBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeCourseFields((CourseResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public Task<ApiResponse<object>> GetCourseByIdAsync(GetCourseByIdRequest request, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.GetByIdAsync(
            request.Id,
            request.GetEffectiveExpandTokens(),
            _courseRepository.GetByIdAsync,
            EntityToBusinessMapper.ToCourseBusiness,
            BusinessToResponseMapper.ToCourseResponse,
            "Course not found",
            cancellationToken);

    public async Task<ApiResponse<object>> GetCourseEnrollmentsAsync(
        int courseId,
        GetCourseEnrollmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _courseRepository.GetByIdAsync(courseId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ApiResponse<object>.Fail("Course not found");
        }

        var query = _enrollmentRepository
            .QueryEnrollments(request.GetExpandTokens())
            .Where(e => e.CourseId == courseId);

        var result = await PagedListResultBuilder.CreateAsync(
            request,
            query,
            q => ApplyEnrollmentSearch(q, request.Search),
            EnrollmentSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToEnrollmentResponse(
                EntityToBusinessMapper.ToEnrollmentBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeEnrollmentFields((EnrollmentResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public async Task<ApiResponse<object>> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        if (await _semesterRepository.GetByIdAsync(request.SemesterId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("semesterId", "Semester does not exist.");
        }

        if (await _subjectRepository.GetByIdAsync(request.SubjectId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("subjectId", "Subject does not exist.");
        }

        var entity = new Course
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId,
            SubjectId = request.SubjectId
        };

        var created = await _courseRepository.AddAsync(entity, cancellationToken);
        var response = BusinessToResponseMapper.ToCourseResponse(
            EntityToBusinessMapper.ToCourseBusiness(created, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource created successfully");
    }

    public async Task<ApiResponse<object>> UpdateCourseAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _courseRepository.FindTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail("Course not found");
        }

        if (await _semesterRepository.GetByIdAsync(request.SemesterId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("semesterId", "Semester does not exist.");
        }

        if (await _subjectRepository.GetByIdAsync(request.SubjectId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("subjectId", "Subject does not exist.");
        }

        entity.CourseName = request.CourseName.Trim();
        entity.SemesterId = request.SemesterId;
        entity.SubjectId = request.SubjectId;

        await _courseRepository.SaveChangesAsync(cancellationToken);

        var response = BusinessToResponseMapper.ToCourseResponse(
            EntityToBusinessMapper.ToCourseBusiness(entity, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource updated successfully");
    }

    public Task<ApiResponse<object>> DeleteCourseAsync(int id, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.DeleteAsync(
            id,
            _courseRepository.DeleteByIdAsync,
            "Course not found",
            cancellationToken);

    private static IQueryable<Enrollment> ApplyEnrollmentSearch(IQueryable<Enrollment> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(e =>
            e.Status.ToLower().Contains(keyword) ||
            e.Student.FullName.ToLower().Contains(keyword) ||
            e.Student.Email.ToLower().Contains(keyword));
    }

    private static IQueryable<Course> ApplySearch(IQueryable<Course> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(c =>
            c.CourseName.ToLower().Contains(keyword) ||
            c.Subject.SubjectCode.ToLower().Contains(keyword) ||
            c.Subject.SubjectName.ToLower().Contains(keyword) ||
            c.Semester.SemesterName.ToLower().Contains(keyword));
    }
}
