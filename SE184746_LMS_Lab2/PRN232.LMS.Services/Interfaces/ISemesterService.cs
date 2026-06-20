using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<(IEnumerable<SemesterBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default);

    Task<SemesterBusiness?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default);

    Task<SemesterBusiness> CreateAsync(SemesterBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, SemesterBusiness model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
