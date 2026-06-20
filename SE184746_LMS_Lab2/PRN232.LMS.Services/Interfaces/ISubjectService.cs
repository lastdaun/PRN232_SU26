using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<(IEnumerable<SubjectBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default);

    Task<SubjectBusiness?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default);

    Task<SubjectBusiness> CreateAsync(SubjectBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, SubjectBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
