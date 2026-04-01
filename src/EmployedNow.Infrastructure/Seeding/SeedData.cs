using EmployedNow.Domain.Entities;
using EmployedNow.Domain.Enums;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Seeding;

public static class SeedData
{
    /// <summary>
    /// Seeds deterministic sample data once for local MVP/demo usage.
    /// </summary>
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        // Prevents duplicate seed records on repeated application starts.
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var company = new User
        {
            Email = "company@employednow.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Company123!"),
            Role = UserRole.Company,
            IsPremium = false
        };

        var seeker1 = new User
        {
            Email = "seeker1@employednow.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seeker123!"),
            Role = UserRole.User,
            IsPremium = false
        };

        var seeker2 = new User
        {
            Email = "seeker2@employednow.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seeker123!"),
            Role = UserRole.User,
            IsPremium = false
        };

        dbContext.Users.AddRange(company, seeker1, seeker2);
        await dbContext.SaveChangesAsync();

        var job1 = new JobPosting
        {
            CompanyId = company.Id,
            Title = "Backend .NET Developer",
            Description = "Build and maintain APIs for EmployedNow."
        };

        var job2 = new JobPosting
        {
            CompanyId = company.Id,
            Title = "Frontend React Developer",
            Description = "Create responsive user interfaces for the platform."
        };

        dbContext.JobPostings.AddRange(job1, job2);
        await dbContext.SaveChangesAsync();

        dbContext.JobApplications.AddRange(
            new JobApplication { JobPostingId = job1.Id, UserId = seeker1.Id },
            new JobApplication { JobPostingId = job2.Id, UserId = seeker2.Id });

        dbContext.Connections.AddRange(
            new Connection
            {
                RequesterId = seeker1.Id,
                TargetId = seeker2.Id,
                Status = ConnectionStatus.Pending
            },
            new Connection
            {
                RequesterId = seeker2.Id,
                TargetId = seeker1.Id,
                Status = ConnectionStatus.Accepted
            });

        await dbContext.SaveChangesAsync();
    }
}
