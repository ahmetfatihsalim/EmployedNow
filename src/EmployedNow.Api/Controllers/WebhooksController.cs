using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployedNow.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IPremiumService _premiumService;
    private readonly IConfiguration _configuration;

    public WebhooksController(IPremiumService premiumService, IConfiguration configuration)
    {
        _premiumService = premiumService;
        _configuration = configuration;
    }

    /// <summary>
    /// Processes premium status updates from external payment gateway webhooks.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("premium")]
    public async Task<IActionResult> PremiumUpgrade(
        [FromHeader(Name = "X-Webhook-Secret")] string? secret,
        [FromBody] PremiumWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var configuredSecret = _configuration["Webhook:PremiumSecret"];
        // Rejects requests unless they include the agreed shared secret.
        if (string.IsNullOrWhiteSpace(configuredSecret) || secret != configuredSecret)
        {
            throw new ApiException("Invalid webhook secret.", 401);
        }

        await _premiumService.UpgradeAsync(request, cancellationToken);
        return Ok(new { message = "Premium status updated." });
    }
}
