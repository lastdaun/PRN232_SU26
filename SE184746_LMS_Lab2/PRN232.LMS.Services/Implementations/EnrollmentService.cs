using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;
    private readonly IStudentRepository _studentRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IMapper _mapper;

    public EnrollmentService(
        IEnrollmentRepository repo,
        IStudentRepository studentRepo,
        ICourseRepository courseRepo,
        IMapper mapper)
    {
        _repo = repo;
        _studentRepo = studentRepo;
        _courseRepo = courseRepo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<EnrollmentBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.GetAllAsync(search, sort, page, size, includeStudent, includeCourse, cancellationToken);
        var business = _mapper.Map<IEnumerable<EnrollmentBusiness>>(items);
        foreach (var b in business)
        {
            if (!includeStudent) b.Student = null;
            if (!includeCourse) b.Course = null;
        }
        return (business, total);
    }

    public async Task<EnrollmentBusiness?> GetByIdAsync(int id,
        bool includeStudent = false, bool includeCourse = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeStudent, includeCourse, cancellationToken);
        if (entity == null) return null;
        var business = _mapper.Map<EnrollmentBusiness>(entity);
        if (!includeStudent) business.Student = null;
        if (!includeCourse) business.Course = null;
        return business;
    }

    public async Task<EnrollmentBusiness> CreateAsync(EnrollmentBusiness model,
        CancellationToken cancellationToken = default)
    {
        await EnsureStudentExistsAsync(model.StudentId, cancellationToken);
        await EnsureCourseExistsAsync(model.CourseId, cancellationToken);

        if (await _repo.ExistsByStudentAndCourseAsync(model.StudentId, model.CourseId, cancellationToken: cancellationToken))
            throw new ResourceValidationException(
                $"Student {model.StudentId} is already enrolled in course {model.CourseId}.");

        var entity = _mapper.Map<Enrollment>(model);
        await _repo.AddAsync(entity, cancellationToken);
        try
        {
            await _repo.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateViolation(ex))
        {
            throw new ResourceValidationException(
                $"Student {model.StudentId} is already enrolled in course {model.CourseId}.");
        }
        model.EnrollmentId = entity.EnrollmentId;
        return model;
    }

    public async Task<bool> UpdateAsync(int id, EnrollmentBusiness model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;

        await EnsureStudentExistsAsync(model.StudentId, cancellationToken);
        await EnsureCourseExistsAsync(model.CourseId, cancellationToken);

        if (await _repo.ExistsByStudentAndCourseAsync(model.StudentId, model.CourseId, excludeId: id, cancellationToken: cancellationToken))
            throw new ResourceValidationException(
                $"Student {model.StudentId} is already enrolled in course {model.CourseId}.");

        _mapper.Map(model, entity);
        await _repo.UpdateAsync(entity);
        try
        {
            return await _repo.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateViolation(ex))
        {
            throw new ResourceValidationException(
                $"Student {model.StudentId} is already enrolled in course {model.CourseId}.");
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;
        await _repo.DeleteAsync(entity);
        return await _repo.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStudentExistsAsync(int studentId, CancellationToken ct)
    {
        if (await _studentRepo.GetByIdAsync(studentId, cancellationToken: ct) == null)
            throw new ResourceNotFoundException($"Student {studentId} not found.");
    }

    private async Task EnsureCourseExistsAsync(int courseId, CancellationToken ct)
    {
        if (await _courseRepo.GetByIdAsync(courseId, cancellationToken: ct) == null)
            throw new ResourceNotFoundException($"Course {courseId} not found.");
    }

    private static bool IsDuplicateViolation(DbUpdateException ex) =>
        ex.InnerException is Microsoft.Data.SqlClient.SqlException sql
        && (sql.Number == 2627 || sql.Number == 2601);
}
