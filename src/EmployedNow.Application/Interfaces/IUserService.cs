using EmployedNow.Application.DTOs;
using EmployedNow.Domain.Enums;

namespace EmployedNow.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, UserRole? role, CancellationToken cancellationToken = default);
    Task<UserProfileResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileResponse> UpdatePremiumAsync(Guid userId, bool isPremium, CancellationToken cancellationToken = default);
}
