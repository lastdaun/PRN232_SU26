using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;
    private readonly IMapper _mapper;

    public SubjectService(ISubjectRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<SubjectBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size, bool includeCourses,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.GetAllAsync(search, sort, page, size, includeCourses, cancellationToken);
        var business = _mapper.Map<IEnumerable<SubjectBusiness>>(items);
        if (!includeCourses)
            foreach (var b in business) b.Courses = null;
        return (business, total);
    }

    public async Task<SubjectBusiness?> GetByIdAsync(int id, bool includeCourses = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeCourses, cancellationToken);
        if (entity == null) return null;
        var business = _mapper.Map<SubjectBusiness>(entity);
        if (!includeCourses) business.Courses = null;
        return business;
    }

    public async Task<SubjectBusiness> CreateAsync(SubjectBusiness model,
        CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Subject>(model);
        await _repo.AddAsync(entity, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        model.SubjectId = entity.SubjectId;
        return model;
    }

    public async Task<bool> UpdateAsync(int id, SubjectBusiness model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null) return false;
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
