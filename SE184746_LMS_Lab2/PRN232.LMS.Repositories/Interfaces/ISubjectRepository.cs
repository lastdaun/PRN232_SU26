using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository
{
    Task<(IEnumerable<Subject> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default);

    Task<Subject?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(Subject entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject entity);
    Task DeleteAsync(Subject entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
