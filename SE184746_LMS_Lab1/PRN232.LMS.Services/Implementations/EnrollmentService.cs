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

public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ApiResponse<object>> GetEnrollmentsAsync(GetEnrollmentsRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PagedListResultBuilder.CreateAsync(
            request,
            _enrollmentRepository.QueryEnrollments(request.GetExpandTokens()),
            query => ApplySearch(query, request.Search),
            EnrollmentSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToEnrollmentResponse(
                EntityToBusinessMapper.ToEnrollmentBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeEnrollmentFields((EnrollmentResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public Task<ApiResponse<object>> GetEnrollmentByIdAsync(GetEnrollmentByIdRequest request, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.GetByIdAsync(
            request.Id,
            request.GetEffectiveExpandTokens(),
            _enrollmentRepository.GetByIdAsync,
            EntityToBusinessMapper.ToEnrollmentBusiness,
            BusinessToResponseMapper.ToEnrollmentResponse,
            "Enrollment not found",
            cancellationToken);

    public async Task<ApiResponse<object>> CreateEnrollmentAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _studentRepository.GetByIdAsync(request.StudentId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("studentId", "Student does not exist.");
        }

        if (await _courseRepository.GetByIdAsync(request.CourseId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("courseId", "Course does not exist.");
        }

        var entity = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status.Trim()
        };

        var created = await _enrollmentRepository.AddAsync(entity, cancellationToken);
        var response = BusinessToResponseMapper.ToEnrollmentResponse(
            EntityToBusinessMapper.ToEnrollmentBusiness(created, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource created successfully");
    }

    public async Task<ApiResponse<object>> UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _enrollmentRepository.FindTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail("Enrollment not found");
        }

        if (await _studentRepository.GetByIdAsync(request.StudentId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("studentId", "Student does not exist.");
        }

        if (await _courseRepository.GetByIdAsync(request.CourseId, Array.Empty<string>(), cancellationToken) is null)
        {
            return ResourceServiceHelper.ValidationFail("courseId", "Course does not exist.");
        }

        entity.StudentId = request.StudentId;
        entity.CourseId = request.CourseId;
        entity.EnrollDate = request.EnrollDate;
        entity.Status = request.Status.Trim();

        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        var response = BusinessToResponseMapper.ToEnrollmentResponse(
            EntityToBusinessMapper.ToEnrollmentBusiness(entity, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource updated successfully");
    }

    public Task<ApiResponse<object>> DeleteEnrollmentAsync(int id, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.DeleteAsync(
            id,
            _enrollmentRepository.DeleteByIdAsync,
            "Enrollment not found",
            cancellationToken);

    private static IQueryable<Enrollment> ApplySearch(IQueryable<Enrollment> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(e =>
            e.Status.ToLower().Contains(keyword) ||
            e.Student.FullName.ToLower().Contains(keyword) ||
            e.Student.Email.ToLower().Contains(keyword) ||
            e.Course.CourseName.ToLower().Contains(keyword));
    }
}
