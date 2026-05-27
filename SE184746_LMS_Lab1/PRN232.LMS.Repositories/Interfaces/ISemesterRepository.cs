using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository
{
    IQueryable<Semester> QuerySemesters(IEnumerable<string> expand);
    Task<Semester?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default);
    Task<Semester> AddAsync(Semester semester, CancellationToken cancellationToken = default);
    Task<Semester?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}
