using EmployedNow.Api.Extensions;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployedNow.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// Returns public paginated job listings.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResult<JobSummaryResponse>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _jobService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns details for a single public job posting.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{jobId:guid}")]
    public async Task<ActionResult<JobDetailResponse>> GetById(Guid jobId, CancellationToken cancellationToken)
    {
        var response = await _jobService.GetByIdAsync(jobId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Creates a job posting for the authenticated company account.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.Company))]
    [HttpPost]
    public async Task<ActionResult<JobSummaryResponse>> Create([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var response = await _jobService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { jobId = response.Id }, response);
    }
}
