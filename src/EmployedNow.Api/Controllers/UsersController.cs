using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Api.Extensions;
using EmployedNow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployedNow.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns paginated users for discoverability in networking UI.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserSummaryResponse>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] UserRole? role = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _userService.GetPagedAsync(pageNumber, pageSize, role, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns profile details for the authenticated user.
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMe(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _userService.GetByIdAsync(userId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Updates premium status for the authenticated user.
    /// </summary>
    [HttpPut("me/premium")]
    public async Task<ActionResult<UserProfileResponse>> UpdatePremium([FromBody] UpdatePremiumRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _userService.UpdatePremiumAsync(userId, request.IsPremium, cancellationToken);
        return Ok(response);
    }
}
