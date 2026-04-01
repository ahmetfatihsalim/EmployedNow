using EmployedNow.Api.Extensions;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployedNow.Api.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Applies the authenticated user to a job posting.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.User))]
    [HttpPost]
    public async Task<ActionResult<JobApplicationResponse>> Apply([FromBody] CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _applicationService.ApplyAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns applications for a job owned by the authenticated company.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.Company))]
    [HttpGet("job/{jobId:guid}")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationResponse>>> GetByJob(Guid jobId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _applicationService.GetByJobForCompanyAsync(userId, jobId, cancellationToken);
        return Ok(response);
    }
}
