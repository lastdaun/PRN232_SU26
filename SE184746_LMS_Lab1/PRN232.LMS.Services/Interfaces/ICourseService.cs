using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<ApiResponse<object>> GetCoursesAsync(GetCoursesRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetCourseByIdAsync(GetCourseByIdRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetCourseEnrollmentsAsync(int courseId, GetCourseEnrollmentsRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateCourseAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteCourseAsync(int id, CancellationToken cancellationToken = default);
}
