using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Helpers;

public static class ResourceServiceHelper
{
    public static async Task<ApiResponse<object>> GetByIdAsync<TEntity, TBusiness>(
        int id,
        IReadOnlyList<string> expand,
        Func<int, IEnumerable<string>, CancellationToken, Task<TEntity?>> getById,
        Func<TEntity, IReadOnlyCollection<string>, TBusiness> toBusiness,
        Func<TBusiness, object> toResponse,
        string notFoundMessage,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var entity = await getById(id, expand, cancellationToken);
        if (entity is null)
        {
            return ApiResponse<object>.Fail(notFoundMessage);
        }

        return ApiResponse<object>.Ok(toResponse(toBusiness(entity, expand)));
    }

    public static ApiResponse<object> ValidationFail(string field, string message)
        => ApiResponse<object>.Fail(
            "Validation failed.",
            new Dictionary<string, string[]> { [field] = [message] });

    public static async Task<ApiResponse<object>> DeleteAsync(
        int id,
        Func<int, CancellationToken, Task<bool>> deleteById,
        string notFoundMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await deleteById(id, cancellationToken))
            {
                return ApiResponse<object>.Fail(notFoundMessage);
            }

            return ApiResponse<object>.Ok(new { id }, "Resource deleted successfully");
        }
        catch (DbUpdateException exception)
        {
            return PersistenceHelper.TryMapDeleteConflict(exception)
                ?? ApiResponse<object>.Fail("Unable to delete resource.");
        }
    }
}
