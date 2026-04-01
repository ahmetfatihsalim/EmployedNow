using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class PremiumService : IPremiumService
{
    private readonly AppDbContext _dbContext;

    public PremiumService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Updates premium status for a user based on verified webhook payload.
    /// </summary>
    public async Task UpgradeAsync(PremiumWebhookRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            throw new ApiException("User not found.", 404);
        }

        user.IsPremium = request.Upgrade;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
