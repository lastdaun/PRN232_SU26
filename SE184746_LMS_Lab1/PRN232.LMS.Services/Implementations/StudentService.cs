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

public sealed class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<ApiResponse<object>> GetStudentsAsync(GetStudentsRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PagedListResultBuilder.CreateAsync(
            request,
            _studentRepository.QueryStudents(request.GetExpandTokens()),
            query => ApplySearch(query, request.Search),
            StudentSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToStudentResponse(
                EntityToBusinessMapper.ToStudentBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeStudentFields((StudentResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public Task<ApiResponse<object>> GetStudentByIdAsync(GetStudentByIdRequest request, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.GetByIdAsync(
            request.Id,
            request.GetEffectiveExpandTokens(),
            _studentRepository.GetByIdAsync,
            EntityToBusinessMapper.ToStudentBusiness,
            BusinessToResponseMapper.ToStudentResponse,
            "Student not found",
            cancellationToken);

    public async Task<ApiResponse<object>> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _studentRepository.EmailExistsAsync(request.Email.Trim(), cancellationToken: cancellationToken))
        {
            return ResourceServiceHelper.ValidationFail("email", "Email already exists.");
        }

        var entity = new Student
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            DateOfBirth = request.DateOfBirth
        };

        var created = await _studentRepository.AddAsync(entity, cancellationToken);
        var response = BusinessToResponseMapper.ToStudentResponse(
            EntityToBusinessMapper.ToStudentBusiness(created, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource created successfully");
    }

    public async Task<ApiResponse<object>> UpdateStudentAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _studentRepository.FindTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail("Student not found");
        }

        var email = request.Email.Trim();
        if (await _studentRepository.EmailExistsAsync(email, id, cancellationToken))
        {
            return ResourceServiceHelper.ValidationFail("email", "Email already exists.");
        }

        entity.FullName = request.FullName.Trim();
        entity.Email = email;
        entity.DateOfBirth = request.DateOfBirth;

        await _studentRepository.SaveChangesAsync(cancellationToken);

        var response = BusinessToResponseMapper.ToStudentResponse(
            EntityToBusinessMapper.ToStudentBusiness(entity, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource updated successfully");
    }

    public Task<ApiResponse<object>> DeleteStudentAsync(int id, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.DeleteAsync(
            id,
            _studentRepository.DeleteByIdAsync,
            "Student not found",
            cancellationToken);

    private static IQueryable<Student> ApplySearch(IQueryable<Student> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(s =>
            s.FullName.ToLower().Contains(keyword) ||
            s.Email.ToLower().Contains(keyword));
    }
}
