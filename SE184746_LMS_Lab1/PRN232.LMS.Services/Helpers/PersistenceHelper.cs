using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Helpers;

public static class PersistenceHelper
{
    public static bool IsNotFoundMessage(string message)
        => message.Contains("not found", StringComparison.OrdinalIgnoreCase);

    public static ApiResponse<object>? TryMapDeleteConflict(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        if (message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase)
            || message.Contains("foreign key", StringComparison.OrdinalIgnoreCase))
        {
            return ApiResponse<object>.Fail(
                "Cannot delete resource because related data exists.",
                new Dictionary<string, string[]> { ["id"] = ["Delete is blocked by existing related records."] });
        }

        return null;
    }
}
