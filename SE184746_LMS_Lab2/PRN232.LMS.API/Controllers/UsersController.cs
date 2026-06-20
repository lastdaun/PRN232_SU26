using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.API.Controllers;

/// <summary>User management – Admin only.</summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize(Roles = "Admin")]
public sealed class UsersController : LmsControllerBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;

    public UsersController(IUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>Get a paged list of users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryRequest query, CancellationToken ct)
    {
        var (items, total) = await _service.GetAllAsync(
            query.Search, query.Sort, query.Page, query.Size, ct);

        var responses = items.Select(u => _mapper.Map<UserResponse>(u));
        return Ok(ApiResponse<PaginatedResponse<UserResponse>>.Ok(
            ToPaginatedResponse(responses, total, query.Page, query.Size)));
    }

    /// <summary>Get a user by ID.</summary>
    [HttpGet("{id:int}", Name = "GetUserById")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var user = await _service.GetByIdAsync(id, ct);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail($"User {id} not found."));

        return Ok(ApiResponse<UserResponse>.Ok(_mapper.Map<UserResponse>(user)));
    }

    /// <summary>Create a new user (Admin only).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(_mapper.Map<UserBusiness>(request), ct);
        var response = _mapper.Map<UserResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.UserId },
            ApiResponse<UserResponse>.Ok(response, "User created successfully."));
    }

    /// <summary>Update a user (Admin only).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var model = _mapper.Map<UserBusiness>(request);
        var updated = await _service.UpdateAsync(id, model, ct);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"User {id} not found."));

        var user = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<UserResponse>.Ok(_mapper.Map<UserResponse>(user!), "User updated successfully."));
    }

    /// <summary>Delete a user (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"User {id} not found."));

        return NoContent();
    }
}
