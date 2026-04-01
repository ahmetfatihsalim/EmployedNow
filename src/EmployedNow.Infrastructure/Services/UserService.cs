using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Application.Common;
using EmployedNow.Domain.Enums;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns paginated users for networking discovery and role-based filtering.
    /// </summary>
    public async Task<PagedResult<UserSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, UserRole? role, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _dbContext.Users.AsNoTracking().AsQueryable();
        if (role.HasValue)
        {
            query = query.Where(x => x.Role == role.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UserSummaryResponse(x.Id, x.Email, x.Role, x.IsPremium, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserSummaryResponse>(items, pageNumber, pageSize, total);
    }

    /// <summary>
    /// Returns profile details for the current authenticated user.
    /// </summary>
    public async Task<UserProfileResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (entity is null)
        {
            throw new ApiException("User not found.", 404);
        }

        return new UserProfileResponse(entity.Id, entity.Email, entity.Role, entity.IsPremium, entity.CreatedAt);
    }

    /// <summary>
    /// Updates premium status for the current authenticated user.
    /// </summary>
    public async Task<UserProfileResponse> UpdatePremiumAsync(Guid userId, bool isPremium, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (entity is null)
        {
            throw new ApiException("User not found.", 404);
        }

        entity.IsPremium = isPremium;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(entity.Id, entity.Email, entity.Role, entity.IsPremium, entity.CreatedAt);
    }
}
