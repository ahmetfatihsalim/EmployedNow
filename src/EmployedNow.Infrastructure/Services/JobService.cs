using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Enums;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _dbContext;

    public JobService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new job posting owned by a company account.
    /// </summary>
    public async Task<JobSummaryResponse> CreateAsync(Guid companyId, CreateJobRequest request, CancellationToken cancellationToken = default)
    {
        var company = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken);
        if (company is null)
        {
            throw new ApiException("Company not found.", 404);
        }

        if (company.Role != UserRole.Company)
        {
            throw new ApiException("Only company accounts can create job posts.", 403);
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ApiException("Title and description are required.", 400);
        }

        // Trims payload values so list/detail output remains normalized.
        var entity = new Domain.Entities.JobPosting
        {
            CompanyId = companyId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim()
        };

        _dbContext.JobPostings.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new JobSummaryResponse(entity.Id, entity.Title, entity.Description, entity.CompanyId, entity.CreatedAt);
    }

    /// <summary>
    /// Returns public job postings with bounded pagination parameters.
    /// </summary>
    public async Task<PagedResult<JobSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        // Applies defensive defaults and cap to prevent unbounded reads.
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

        var query = _dbContext.JobPostings
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new JobSummaryResponse(x.Id, x.Title, x.Description, x.CompanyId, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<JobSummaryResponse>(items, pageNumber, pageSize, total);
    }

    /// <summary>
    /// Returns a single public job detail by its identifier.
    /// </summary>
    public async Task<JobDetailResponse> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.JobPostings
            .AsNoTracking()
            .Where(x => x.Id == jobId)
            .Select(x => new JobDetailResponse(x.Id, x.Title, x.Description, x.CompanyId, x.Company.Email, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (job is null)
        {
            throw new ApiException("Job not found.", 404);
        }

        return job;
    }
}
