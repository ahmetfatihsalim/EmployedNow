using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace EmployedNow.Api.IntegrationTests;

/// <summary>
/// Validates core API behaviors required for MVP acceptance.
/// </summary>
public class ApiSmokeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(CustomWebApplicationFactory factory)
    {
        // Creates an in-memory test server client for endpoint verification.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Should_Return_Ok()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Public_Jobs_List_Should_Be_Accessible_Without_Token()
    {
        var response = await _client.GetAsync("/api/jobs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Protected_Endpoint_Should_Reject_Anonymous_User()
    {
        var response = await _client.PostAsJsonAsync("/api/jobs", new { title = "Test", description = "Test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_With_Seeded_Company_Should_Return_Jwt()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "company@employednow.local",
            password = "Company123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        payload.Should().NotBeNull();
        payload!.Token.Should().NotBeNullOrWhiteSpace();
    }

    private sealed class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
