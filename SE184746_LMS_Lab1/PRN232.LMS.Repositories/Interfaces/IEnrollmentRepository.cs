using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    IQueryable<Enrollment> QueryEnrollments(IEnumerable<string> expand);
    Task<Enrollment?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default);
    Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Enrollment?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}
