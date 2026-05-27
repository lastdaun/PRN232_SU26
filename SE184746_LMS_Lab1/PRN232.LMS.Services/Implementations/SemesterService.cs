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

public sealed class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _semesterRepository;

    public SemesterService(ISemesterRepository semesterRepository)
    {
        _semesterRepository = semesterRepository;
    }

    public async Task<ApiResponse<object>> GetSemestersAsync(GetSemestersRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PagedListResultBuilder.CreateAsync(
            request,
            _semesterRepository.QuerySemesters(request.GetExpandTokens()),
            query => ApplySearch(query, request.Search),
            SemesterSorting.Apply,
            (entity, expand) => BusinessToResponseMapper.ToSemesterResponse(
                EntityToBusinessMapper.ToSemesterBusiness(entity, expand)),
            (item, fields) => BusinessToResponseMapper.ShapeSemesterFields((SemesterResponse)item, fields),
            cancellationToken);

        return ApiResponse<object>.Ok(result);
    }

    public Task<ApiResponse<object>> GetSemesterByIdAsync(GetSemesterByIdRequest request, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.GetByIdAsync(
            request.Id,
            request.GetEffectiveExpandTokens(),
            _semesterRepository.GetByIdAsync,
            EntityToBusinessMapper.ToSemesterBusiness,
            BusinessToResponseMapper.ToSemesterResponse,
            "Semester not found",
            cancellationToken);

    public async Task<ApiResponse<object>> CreateSemesterAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Semester
        {
            SemesterName = request.SemesterName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var created = await _semesterRepository.AddAsync(entity, cancellationToken);
        var response = BusinessToResponseMapper.ToSemesterResponse(
            EntityToBusinessMapper.ToSemesterBusiness(created, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource created successfully");
    }

    public async Task<ApiResponse<object>> UpdateSemesterAsync(int id, UpdateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _semesterRepository.FindTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail("Semester not found");
        }

        entity.SemesterName = request.SemesterName.Trim();
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;

        await _semesterRepository.SaveChangesAsync(cancellationToken);

        var response = BusinessToResponseMapper.ToSemesterResponse(
            EntityToBusinessMapper.ToSemesterBusiness(entity, Array.Empty<string>()));

        return ApiResponse<object>.Ok(response, "Resource updated successfully");
    }

    public Task<ApiResponse<object>> DeleteSemesterAsync(int id, CancellationToken cancellationToken = default)
        => ResourceServiceHelper.DeleteAsync(
            id,
            _semesterRepository.DeleteByIdAsync,
            "Semester not found",
            cancellationToken);

    private static IQueryable<Semester> ApplySearch(IQueryable<Semester> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim().ToLower();
        return query.Where(s => s.SemesterName.ToLower().Contains(keyword));
    }
}
