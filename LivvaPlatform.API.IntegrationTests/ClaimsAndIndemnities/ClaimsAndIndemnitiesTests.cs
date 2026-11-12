using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.ClaimsAndIndemnities;

/// <summary>
///     Integration tests for the Claims and Indemnities endpoints used by the web application.
/// </summary>
public class ClaimsAndIndemnitiesTests(LivvaWebApplicationFactory factory) : IClassFixture<LivvaWebApplicationFactory>
{
    private async Task<(HttpClient Client, long UserId)> AuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var email = $"user{Guid.NewGuid():N}@livva.pe";
        await client.PostAsJsonAsync("/api/v1/authentication/sign-up",
            new { firstName = "Ana", lastName = "Torres", email, password = "secret123" });
        var signIn = await client.PostAsJsonAsync("/api/v1/authentication/sign-in", new { email, password = "secret123" });
        var body = await signIn.Content.ReadFromJsonAsync<JsonElement>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body.GetProperty("token").GetString());
        return (client, body.GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task VehicleClaim_ShouldBeReportedListedAndCancelledWithHistory()
    {
        var (client, userId) = await AuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/vehicle-claims", new
        {
            policyId = 1, incidentDate = "2026-10-05", incidentLocation = "Av. Arequipa, Lima",
            description = "El vehículo sufrió un impacto leve en la parte posterior.", estimatedDamageAmount = 2500,
            status = "approved"
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var claim = await create.Content.ReadFromJsonAsync<JsonElement>();
        var claimId = claim.GetProperty("id").GetInt64();
        Assert.Equal("reported", claim.GetProperty("status").GetString());
        Assert.Equal(userId, claim.GetProperty("userId").GetInt64());
        Assert.Equal("Av. Arequipa, Lima", claim.GetProperty("incidentLocation").GetString());

        var mine = await client.GetFromJsonAsync<JsonElement>("/api/v1/vehicle-claims");
        Assert.Equal(1, mine.GetArrayLength());

        var cancel = await client.PatchAsJsonAsync($"/api/v1/vehicle-claims/{claimId}", new { status = "cancelled" });
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal("cancelled", (await cancel.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("status").GetString());

        var history = await client.GetFromJsonAsync<JsonElement>($"/api/v1/claim-status-histories?claimId={claimId}");
        Assert.Equal(2, history.GetArrayLength());
        Assert.Equal("cancelled", history[0].GetProperty("status").GetString());

        var again = await client.PatchAsJsonAsync($"/api/v1/vehicle-claims/{claimId}", new { status = "under_review" });
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task VehicleClaim_WithInvalidStatusOrPolicy_ShouldFail()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var missingPolicy = await client.PostAsJsonAsync("/api/v1/vehicle-claims", new
        {
            policyId = 9999, incidentDate = "2026-10-05", incidentLocation = "Lima", description = "Choque leve.",
            estimatedDamageAmount = 100
        });
        Assert.Equal(HttpStatusCode.NotFound, missingPolicy.StatusCode);

        var invalidStatus = await client.PatchAsJsonAsync("/api/v1/vehicle-claims/1", new { status = "unknown" });
        Assert.Equal(HttpStatusCode.BadRequest, invalidStatus.StatusCode);
    }
}
