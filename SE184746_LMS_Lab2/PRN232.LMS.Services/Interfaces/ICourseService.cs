using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<(IEnumerable<CourseBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeSemester, bool includeSubject, bool includeEnrollments,
        CancellationToken cancellationToken = default);

    Task<CourseBusiness?> GetByIdAsync(int id,
        bool includeSemester = false, bool includeSubject = false, bool includeEnrollments = false,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<EnrollmentBusiness> Items, int Total)> GetCourseEnrollmentsAsync(
        int courseId, string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default);

    Task<CourseBusiness> CreateAsync(CourseBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, CourseBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
