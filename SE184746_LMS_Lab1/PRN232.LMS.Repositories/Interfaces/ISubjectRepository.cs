using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository
{
    IQueryable<Subject> QuerySubjects(IEnumerable<string> expand);
    Task<Subject?> GetByIdAsync(int id, IEnumerable<string> expand, CancellationToken cancellationToken = default);
    Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null, CancellationToken cancellationToken = default);
    Task<Subject> AddAsync(Subject subject, CancellationToken cancellationToken = default);
    Task<Subject?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}
