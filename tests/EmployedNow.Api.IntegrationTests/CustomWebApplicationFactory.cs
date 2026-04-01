using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EmployedNow.Api.IntegrationTests;

/// <summary>
/// Bootstraps the API for integration tests and keeps the SQLite database in a test-local location.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var dbDir = Path.Combine(Path.GetTempPath(), "employednow-tests");
            Directory.CreateDirectory(dbDir);
            var dbPath = Path.Combine(dbDir, "employednow-tests.db");

            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}",
                ["Jwt:Key"] = "EmployedNow_Testing_Key_For_Integration_Tests_Only_2026",
                ["Webhook:PremiumSecret"] = "test-webhook-secret"
            };

            // Overrides configuration so tests always run against isolated local data.
            configBuilder.AddInMemoryCollection(overrides);
        });
    }
}
