using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Interfaces;

public interface IUserService
{
    Task<(IEnumerable<UserBusiness> Items, int Total)> GetAllAsync(
        string? search, string? sort, int page, int size,
        CancellationToken cancellationToken = default);

    Task<UserBusiness?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UserBusiness> CreateAsync(UserBusiness model, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, UserBusiness model, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
