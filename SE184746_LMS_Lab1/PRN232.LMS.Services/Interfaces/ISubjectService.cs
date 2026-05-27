using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<ApiResponse<object>> GetSubjectsAsync(GetSubjectsRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetSubjectByIdAsync(GetSubjectByIdRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> CreateSubjectAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateSubjectAsync(int id, UpdateSubjectRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteSubjectAsync(int id, CancellationToken cancellationToken = default);
}
