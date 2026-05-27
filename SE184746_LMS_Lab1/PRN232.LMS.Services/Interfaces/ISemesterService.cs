using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<ApiResponse<object>> GetSemestersAsync(GetSemestersRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetSemesterByIdAsync(GetSemesterByIdRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> CreateSemesterAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateSemesterAsync(int id, UpdateSemesterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteSemesterAsync(int id, CancellationToken cancellationToken = default);
}
