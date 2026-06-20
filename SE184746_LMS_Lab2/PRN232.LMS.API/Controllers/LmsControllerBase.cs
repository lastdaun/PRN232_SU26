using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Responses;

namespace PRN232.LMS.API.Controllers;

/// <summary>Shared response conventions for LMS API controllers.</summary>
[ApiController]
[Produces("application/json", "application/xml")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public abstract class LmsControllerBase : ControllerBase
{
    protected static PaginatedResponse<T> ToPaginatedResponse<T>(
        IEnumerable<T> items, int total, int page, int size)
        => new()
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                Page = page,
                PageSize = size,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling((double)total / size)
            }
        };
}
