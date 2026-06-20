using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<(IEnumerable<Student> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeEnrollments,
        CancellationToken cancellationToken = default);

    Task<Student?> GetByIdAsync(int id, bool includeEnrollments = false,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Student entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student entity);
    Task DeleteAsync(Student entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
