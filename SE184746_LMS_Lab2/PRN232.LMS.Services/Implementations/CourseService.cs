using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ISemesterRepository _semesterRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IMapper _mapper;

    public CourseService(
        ICourseRepository repo,
        IEnrollmentRepository enrollmentRepo,
        ISemesterRepository semesterRepo,
        ISubjectRepository subjectRepo,
        IMapper mapper)
    {
        _repo = repo;
        _enrollmentRepo = enrollmentRepo;
        _semesterRepo = semesterRepo;
        _subjectRepo = subjectRepo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<CourseBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        bool includeSemester, bool includeSubject, bool includeEnrollments,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.GetAllAsync(
            search, sort, page, size, includeSemester, includeSubject, includeEnrollments, cancellationToken);
        var business = _mapper.Map<IEnumerable<CourseBusiness>>(items);
        foreach (var b in business)
        {
            if (!includeSemester) b.Semester = null;
            if (!includeSubject) b.Subject = null;
            if (!includeEnrollments) b.Enrollments = null;
        }
        return (business, total);
    }

    public async Task<CourseBusiness?> GetByIdAsync(int id,
        bool includeSemester = false, bool includeSubject = false, bool includeEnrollments = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeSemester, includeSubject, includeEnrollments, cancellationToken);
        if (entity == null) return null;
        var business = _mapper.Map<CourseBusiness>(entity);
        if (!includeSemester) business.Semester = null;
        if (!includeSubject) business.Subject = null;
        if (!includeEnrollments) business.Enrollments = null;
        return business;
    }

    public async Task<(IEnumerable<EnrollmentBusiness> Items, int Total)> GetCourseEnrollmentsAsync(
        int courseId, string? search, string? sort, int page, int size,
        bool includeStudent, bool includeCourse,
        CancellationToken cancellationToken = default)
    {
        var courseExists = await _repo.GetByIdAsync(courseId, cancellationToken: cancellationToken);
        if (courseExists == null)
            throw new ResourceNotFoundException($"Course {courseId} not found.");

        var (items, total) = await _enrollmentRepo.GetAllByCourseIdAsync(
            courseId, search, sort, page, size, includeStudent, includeCourse, cancellationToken);

        var business = _mapper.Map<IEnumerable<EnrollmentBusiness>>(items);
        foreach (var b in business)
        {
            if (!includeStudent) b.Student = null;
            if (!includeCourse) b.Course = null;
        }
        return (business, total);
    }

    public async Task<CourseBusiness> CreateAsync(CourseBusiness model,
        CancellationToken cancellationToken = default)
    {
        await EnsureSemesterExistsAsync(model.SemesterId, cancellationToken);
        await EnsureSubjectExistsAsync(model.SubjectId, cancellationToken);

        var entity = _mapper.Map<Course>(model);
        await _repo.AddAsync(entity, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        model.CourseId = entity.CourseId;
        return model;
    }

    public async Task<bool> UpdateAsync(int id, CourseBusiness model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;

        await EnsureSemesterExistsAsync(model.SemesterId, cancellationToken);
        await EnsureSubjectExistsAsync(model.SubjectId, cancellationToken);

        _mapper.Map(model, entity);
        await _repo.UpdateAsync(entity);
        return await _repo.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;
        await _repo.DeleteAsync(entity);
        return await _repo.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSemesterExistsAsync(int semesterId, CancellationToken ct)
    {
        if (await _semesterRepo.GetByIdAsync(semesterId, cancellationToken: ct) == null)
            throw new ResourceNotFoundException($"Semester {semesterId} not found.");
    }

    private async Task EnsureSubjectExistsAsync(int subjectId, CancellationToken ct)
    {
        if (await _subjectRepo.GetByIdAsync(subjectId, cancellationToken: ct) == null)
            throw new ResourceNotFoundException($"Subject {subjectId} not found.");
    }
}
