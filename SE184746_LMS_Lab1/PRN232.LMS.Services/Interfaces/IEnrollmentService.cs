using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<ApiResponse<object>> GetEnrollmentsAsync(GetEnrollmentsRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetEnrollmentByIdAsync(GetEnrollmentByIdRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> CreateEnrollmentAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateEnrollmentAsync(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteEnrollmentAsync(int id, CancellationToken cancellationToken = default);
}
