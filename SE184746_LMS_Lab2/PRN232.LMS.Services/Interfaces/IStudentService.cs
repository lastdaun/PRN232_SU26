using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<(IEnumerable<StudentBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeEnrollments,
        CancellationToken cancellationToken = default);

    Task<StudentBusiness?> GetByIdAsync(int id, bool includeEnrollments = false,
        CancellationToken cancellationToken = default);

    Task<StudentBusiness> CreateAsync(StudentBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, StudentBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
