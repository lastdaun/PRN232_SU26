using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<(IEnumerable<EnrollmentBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default);

    Task<EnrollmentBusiness?> GetByIdAsync(int id,
        bool includeStudent = false, bool includeCourse = false,
        CancellationToken cancellationToken = default);

    Task<EnrollmentBusiness> CreateAsync(EnrollmentBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, EnrollmentBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
