using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<(IEnumerable<Enrollment> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<Enrollment> Items, int Total)> GetAllByCourseIdAsync(
        int courseId, string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default);

    Task<Enrollment?> GetByIdAsync(int id,
        bool includeStudent = false, bool includeCourse = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByStudentAndCourseAsync(int studentId, int courseId, int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Enrollment entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Enrollment entity);
    Task DeleteAsync(Enrollment entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
