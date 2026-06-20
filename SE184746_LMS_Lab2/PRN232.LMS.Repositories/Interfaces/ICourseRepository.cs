using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<(IEnumerable<Course> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeSemester, bool includeSubject, bool includeEnrollments,
        CancellationToken cancellationToken = default);

    Task<Course?> GetByIdAsync(int id,
        bool includeSemester = false, bool includeSubject = false, bool includeEnrollments = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(Course entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course entity);
    Task DeleteAsync(Course entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
