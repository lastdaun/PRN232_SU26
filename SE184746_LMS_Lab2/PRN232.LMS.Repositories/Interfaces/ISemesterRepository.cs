using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository
{
    Task<(IEnumerable<Semester> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default);

    Task<Semester?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(Semester entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Semester entity);
    Task DeleteAsync(Semester entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
