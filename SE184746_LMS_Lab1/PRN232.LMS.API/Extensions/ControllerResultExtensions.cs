using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.API.Extensions;

public static class ControllerResultExtensions
{
    public static IActionResult ToCreatedResult(
        this ControllerBase controller,
        ApiResponse<object> response,
        string getByIdActionName,
        int id)
    {
        if (!response.Success)
        {
            return controller.BadRequest(response);
        }

        return controller.CreatedAtAction(getByIdActionName, new { id }, response);
    }
}
