using EmployedNow.Api.Extensions;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployedNow.Api.Controllers;

[ApiController]
[Route("api/connections")]
[Authorize(Roles = nameof(UserRole.User))]
public class ConnectionsController : ControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionsController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    /// <summary>
    /// Sends a new connection request from the current user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConnectionResponse>> Create([FromBody] CreateConnectionRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _connectionService.CreateRequestAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Accepts a pending connection request addressed to the current user.
    /// </summary>
    [HttpPut("{requestId:guid}")]
    public async Task<ActionResult<ConnectionResponse>> Accept(Guid requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _connectionService.AcceptAsync(userId, requestId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Lists all connection records associated with the current user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConnectionResponse>>> GetForUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _connectionService.GetForUserAsync(userId, cancellationToken);
        return Ok(response);
    }
}
