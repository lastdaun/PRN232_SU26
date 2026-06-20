using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<UserBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.GetAllAsync(search, sort, page, size, cancellationToken);
        return (_mapper.Map<IEnumerable<UserBusiness>>(items), total);
    }

    public async Task<UserBusiness?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<UserBusiness>(entity);
    }

    public async Task<UserBusiness> CreateAsync(UserBusiness model, CancellationToken cancellationToken = default)
    {
        if (await _repo.UsernameExistsAsync(model.Username, cancellationToken: cancellationToken))
            throw new ResourceValidationException($"Username '{model.Username}' is already taken.");

        var entity = new User
        {
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password
                ?? throw new ResourceValidationException("Password is required.")),
            Role = model.Role
        };

        await _repo.AddAsync(entity, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        model.UserId = entity.UserId;
        model.PasswordHash = entity.PasswordHash;
        model.Password = null;
        return model;
    }

    public async Task<bool> UpdateAsync(int id, UserBusiness model, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        if (await _repo.UsernameExistsAsync(model.Username, excludeUserId: id, cancellationToken: cancellationToken))
            throw new ResourceValidationException($"Username '{model.Username}' is already taken.");

        entity.Username = model.Username;
        entity.Role = model.Role;

        if (!string.IsNullOrWhiteSpace(model.Password))
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        await _repo.UpdateAsync(entity);
        return await _repo.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;
        await _repo.DeleteAsync(entity);
        return await _repo.SaveChangesAsync(cancellationToken);
    }
}
