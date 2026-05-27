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

public sealed class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<ApiResponse<object>> GetSubjectsAsync(GetSubjectsRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PagedListResultBuilder.CreateAsync(
            request,
            _subjectRepository.QuerySubjects(request.GetExpandTokens()),
            query => ApplySearch(query, request.Search),
            SubjectSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToSubjectResponse(
                EntityToBusinessMapper.ToSubjectBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeSubjectFields((SubjectResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public Task<ApiResponse<object>> GetSubjectByIdAsync(GetSubjectByIdRequest request, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.GetByIdAsync(
            request.Id,
            request.GetEffectiveExpandTokens(),
            _subjectRepository.GetByIdAsync,
            EntityToBusinessMapper.ToSubjectBusiness,
            BusinessToResponseMapper.ToSubjectResponse,
            "Subject not found",
            cancellationToken);

    public async Task<ApiResponse<object>> CreateSubjectAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        var code = request.SubjectCode.Trim();
        if (await _subjectRepository.SubjectCodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            return ResourceServiceHelper.ValidationFail("subjectCode", "Subject code already exists.");
        }

        var entity = new Subject
        {
            SubjectCode = code,
            SubjectName = request.SubjectName.Trim(),
            Credit = request.Credit
        };

        var created = await _subjectRepository.AddAsync(entity, cancellationToken);
        var response = BusinessToResponseMapper.ToSubjectResponse(
            EntityToBusinessMapper.ToSubjectBusiness(created, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource created successfully");
    }

    public async Task<ApiResponse<object>> UpdateSubjectAsync(int id, UpdateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _subjectRepository.FindTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail("Subject not found");
        }

        var code = request.SubjectCode.Trim();
        if (await _subjectRepository.SubjectCodeExistsAsync(code, id, cancellationToken))
        {
            return ResourceServiceHelper.ValidationFail("subjectCode", "Subject code already exists.");
        }

        entity.SubjectCode = code;
        entity.SubjectName = request.SubjectName.Trim();
        entity.Credit = request.Credit;

        await _subjectRepository.SaveChangesAsync(cancellationToken);

        var response = BusinessToResponseMapper.ToSubjectResponse(
            EntityToBusinessMapper.ToSubjectBusiness(entity, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource updated successfully");
    }

    public Task<ApiResponse<object>> DeleteSubjectAsync(int id, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.DeleteAsync(
            id,
            _subjectRepository.DeleteByIdAsync,
            "Subject not found",
            cancellationToken);

    private static IQueryable<Subject> ApplySearch(IQueryable<Subject> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(s =>
            s.SubjectCode.ToLower().Contains(keyword) ||
            s.SubjectName.ToLower().Contains(keyword));
    }
}
