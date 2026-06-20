using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<StudentBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeEnrollments,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.GetAllAsync(search, sort, page, size, includeEnrollments, cancellationToken);
        var business = _mapper.Map<IEnumerable<StudentBusiness>>(items);
        if (!includeEnrollments)
            foreach (var b in business) b.Enrollments = null;
        return (business, total);
    }

    public async Task<StudentBusiness?> GetByIdAsync(int id, bool includeEnrollments = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeEnrollments, cancellationToken);
        if (entity == null) return null;
        var business = _mapper.Map<StudentBusiness>(entity);
        if (!includeEnrollments) business.Enrollments = null;
        return business;
    }

    public async Task<StudentBusiness> CreateAsync(StudentBusiness model,
        CancellationToken cancellationToken = default)
    {
        if (await _repo.EmailExistsAsync(model.Email, cancellationToken: cancellationToken))
            throw new ResourceValidationException($"Email '{model.Email}' is already in use.");

        var entity = _mapper.Map<Student>(model);
        await _repo.AddAsync(entity, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        model.StudentId = entity.StudentId;
        return model;
    }

    public async Task<bool> UpdateAsync(int id, StudentBusiness model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;

        if (await _repo.EmailExistsAsync(model.Email, excludeStudentId: id, cancellationToken: cancellationToken))
            throw new ResourceValidationException($"Email '{model.Email}' is already in use.");

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
}
