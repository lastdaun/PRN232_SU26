using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<object>> GetStudentsAsync(GetStudentsRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> GetStudentByIdAsync(GetStudentByIdRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UpdateStudentAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteStudentAsync(int id, CancellationToken cancellationToken = default);
}
