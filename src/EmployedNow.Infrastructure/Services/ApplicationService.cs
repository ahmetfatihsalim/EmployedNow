using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Entities;
using EmployedNow.Domain.Enums;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly AppDbContext _dbContext;

    public ApplicationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a job application for a user account and blocks duplicates.
    /// </summary>
    public async Task<JobApplicationResponse> ApplyAsync(Guid userId, CreateApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new ApiException("User not found.", 404);
        }

        if (user.Role != UserRole.User)
        {
            throw new ApiException("Only user accounts can apply to jobs.", 403);
        }

        var job = await _dbContext.JobPostings.FirstOrDefaultAsync(x => x.Id == request.JobId, cancellationToken);
        if (job is null)
        {
            throw new ApiException("Job not found.", 404);
        }

        // Checks business rule before hitting DB unique constraint for clearer API message.
        var duplicate = await _dbContext.JobApplications.AnyAsync(
            x => x.JobPostingId == request.JobId && x.UserId == userId,
            cancellationToken);

        if (duplicate)
        {
            throw new ApiException("You have already applied to this job.", 409);
        }

        var entity = new JobApplication
        {
            JobPostingId = request.JobId,
            UserId = userId
        };

        _dbContext.JobApplications.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationResponse(entity.Id, entity.JobPostingId, entity.UserId, user.Email, entity.AppliedAt);
    }

    /// <summary>
    /// Returns job applications only when the requesting company owns the specified job.
    /// </summary>
    public async Task<IReadOnlyList<JobApplicationResponse>> GetByJobForCompanyAsync(Guid companyId, Guid jobId, CancellationToken cancellationToken = default)
    {
        var company = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken);
        if (company is null)
        {
            throw new ApiException("Company not found.", 404);
        }

        if (company.Role != UserRole.Company)
        {
            throw new ApiException("Only company accounts can view job applications.", 403);
        }

        // Enforces owner-only visibility of applicants for company jobs.
        var ownsJob = await _dbContext.JobPostings.AnyAsync(x => x.Id == jobId && x.CompanyId == companyId, cancellationToken);
        if (!ownsJob)
        {
            throw new ApiException("You are not allowed to view applications for this job.", 403);
        }

        var results = await _dbContext.JobApplications
            .AsNoTracking()
            .Where(x => x.JobPostingId == jobId)
            .OrderByDescending(x => x.AppliedAt)
            .Select(x => new JobApplicationResponse(x.Id, x.JobPostingId, x.UserId, x.User.Email, x.AppliedAt))
            .ToListAsync(cancellationToken);

        return results;
    }
}
