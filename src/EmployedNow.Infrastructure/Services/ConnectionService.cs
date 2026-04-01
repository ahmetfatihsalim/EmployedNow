using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Entities;
using EmployedNow.Domain.Enums;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class ConnectionService : IConnectionService
{
    private readonly AppDbContext _dbContext;

    public ConnectionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Sends a new connection request from requester to target user.
    /// </summary>
    public async Task<ConnectionResponse> CreateRequestAsync(Guid requesterId, CreateConnectionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TargetUserId == requesterId)
        {
            throw new ApiException("You cannot send a connection request to yourself.", 400);
        }

        var requester = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == requesterId, cancellationToken);
        var target = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.TargetUserId, cancellationToken);

        if (requester is null || target is null)
        {
            throw new ApiException("Requester or target user not found.", 404);
        }

          // Blocks duplicate pending requests for the same requester-target pair.
        var pendingExists = await _dbContext.Connections.AnyAsync(
            x => x.RequesterId == requesterId
                 && x.TargetId == request.TargetUserId
                 && x.Status == ConnectionStatus.Pending,
            cancellationToken);

        if (pendingExists)
        {
            throw new ApiException("A pending connection request already exists.", 409);
        }

        var entity = new Connection
        {
            RequesterId = requesterId,
            TargetId = request.TargetUserId,
            Status = ConnectionStatus.Pending
        };

        _dbContext.Connections.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ConnectionResponse(entity.Id, requester.Id, requester.Email, target.Id, target.Email, entity.Status, entity.CreatedAt);
    }

    /// <summary>
    /// Accepts a connection request only when current user is the target user.
    /// </summary>
    public async Task<ConnectionResponse> AcceptAsync(Guid currentUserId, Guid requestId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Connections
            .Include(x => x.Requester)
            .Include(x => x.Target)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (entity is null)
        {
            throw new ApiException("Connection request not found.", 404);
        }

        if (entity.TargetId != currentUserId)
        {
            throw new ApiException("Only the target user can accept this request.", 403);
        }

        // Returning current state keeps operation idempotent for repeated accepts.
        if (entity.Status == ConnectionStatus.Accepted)
        {
            return new ConnectionResponse(entity.Id, entity.RequesterId, entity.Requester.Email, entity.TargetId, entity.Target.Email, entity.Status, entity.CreatedAt);
        }

        entity.Status = ConnectionStatus.Accepted;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ConnectionResponse(entity.Id, entity.RequesterId, entity.Requester.Email, entity.TargetId, entity.Target.Email, entity.Status, entity.CreatedAt);
    }

    /// <summary>
    /// Lists all inbound and outbound connection records for a user.
    /// </summary>
    public async Task<IReadOnlyList<ConnectionResponse>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rows = await _dbContext.Connections
            .AsNoTracking()
            .Include(x => x.Requester)
            .Include(x => x.Target)
            .Where(x => x.RequesterId == userId || x.TargetId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ConnectionResponse(x.Id, x.RequesterId, x.Requester.Email, x.TargetId, x.Target.Email, x.Status, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return rows;
    }
}
