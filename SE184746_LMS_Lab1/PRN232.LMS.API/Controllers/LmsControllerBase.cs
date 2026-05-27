using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Extensions;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.API.Controllers;

/// <summary>Shared helpers and response conventions for LMS API controllers.</summary>
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public abstract class LmsControllerBase : ControllerBase
{
    protected IActionResult OkList(ApiResponse<object> response) => Ok(response);

    protected IActionResult FromGetById(ApiResponse<object> response)
        => response.Success ? Ok(response) : NotFound(response);

    protected IActionResult FromCreate(ApiResponse<object> response, string getByIdActionName, int id)
    {
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return this.ToCreatedResult(response, getByIdActionName, id);
    }

    protected IActionResult FromUpdate(ApiResponse<object> response)
    {
        if (response.Success)
        {
            return Ok(response);
        }

        return PersistenceHelper.IsNotFoundMessage(response.Message)
            ? NotFound(response)
            : BadRequest(response);
    }

    protected IActionResult FromDelete(ApiResponse<object> response)
    {
        if (response.Success)
        {
            return Ok(response);
        }

        return PersistenceHelper.IsNotFoundMessage(response.Message)
            ? NotFound(response)
            : BadRequest(response);
    }

    protected static TByIdRequest ToGetByIdRequest<TByIdRequest>(int id, string? expand)
        where TByIdRequest : GetByIdRequestBase, new()
        => new() { Id = id, Expand = expand };
}
