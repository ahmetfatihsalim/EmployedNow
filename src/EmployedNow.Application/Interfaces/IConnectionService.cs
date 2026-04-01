using EmployedNow.Application.DTOs;

namespace EmployedNow.Application.Interfaces;

public interface IConnectionService
{
    Task<ConnectionResponse> CreateRequestAsync(Guid requesterId, CreateConnectionRequest request, CancellationToken cancellationToken = default);
    Task<ConnectionResponse> AcceptAsync(Guid currentUserId, Guid requestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConnectionResponse>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
