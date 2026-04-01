using System.Security.Claims;
using EmployedNow.Application.Common;

namespace EmployedNow.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the authenticated user identifier from JWT claims.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var id))
        {
            throw new ApiException("Invalid authentication token.", 401);
        }

        return id;
    }
}
