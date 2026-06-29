using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

/// <summary>Authentication endpoints — accessible at /api/auth/... and /api/v{n}/auth/...</summary>
[ApiVersionNeutral]
[Route("api/auth")]
[Route("api/v{version}/auth")]
public sealed class AuthController : LmsControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    /// <summary>Login and receive JWT tokens.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password, ct);
        return Ok(ApiResponse<TokenResponse>.Ok(_mapper.Map<TokenResponse>(token), "Login successful."));
    }

    /// <summary>Refresh an access token using a refresh token.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var token = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
        return Ok(ApiResponse<TokenResponse>.Ok(_mapper.Map<TokenResponse>(token), "Token refreshed successfully."));
    }
}
