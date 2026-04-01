using EmployedNow.Application.DTOs;

namespace EmployedNow.Application.Interfaces;

public interface IPremiumService
{
    Task UpgradeAsync(PremiumWebhookRequest request, CancellationToken cancellationToken = default);
}
