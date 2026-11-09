using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.PolicyManagement;

/// <summary>
///     Integration tests for the Policy Management endpoints.
/// </summary>
public class PolicyManagementTests(LivvaWebApplicationFactory factory) : IClassFixture<LivvaWebApplicationFactory>
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
    public async Task SeededPolicy_ShouldExposeCoveragesAndDocuments()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var policy = await client.GetFromJsonAsync<JsonElement>("/api/v1/policies/1");
        Assert.Equal("LIV-VEH-0001", policy.GetProperty("policyNumber").GetString());
        Assert.Equal("vehicle", policy.GetProperty("insuranceType").GetString());
        Assert.Equal("active", policy.GetProperty("status").GetString());

        var coverages = await client.GetFromJsonAsync<JsonElement>("/api/v1/policy-coverages?policyId=1");
        Assert.Equal(2, coverages.GetArrayLength());
        var documents = await client.GetFromJsonAsync<JsonElement>("/api/v1/policy-documents?policyId=1");
        Assert.Equal(1, documents.GetArrayLength());

        var byUser = await client.GetFromJsonAsync<JsonElement>("/api/v1/policies?userId=1");
        Assert.Equal(3, byUser.GetArrayLength());
    }

    [Fact]
    public async Task Policies_WithoutUserId_ShouldReturnPoliciesOfAuthenticatedUser()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var create = await client.PostAsJsonAsync("/api/v1/policies", new
        {
            userId, applicationId = 10, policyNumber = $"LIV-TST-{Guid.NewGuid():N}"[..20], insuranceType = "life",
            startDate = "2026-05-01", expirationDate = "2027-04-30", premiumAmount = 900, coverageAmount = 90000
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var mine = await client.GetFromJsonAsync<JsonElement>("/api/v1/policies");
        Assert.All(mine.EnumerateArray(), p => Assert.Equal(userId, p.GetProperty("userId").GetInt64()));
        Assert.Equal(1, mine.GetArrayLength());
    }

    [Fact]
    public async Task Renewals_ShouldCreatePendingAndRejectDuplicates()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var create = await client.PostAsJsonAsync("/api/v1/policies", new
        {
            userId, applicationId = 11, policyNumber = $"LIV-REN-{Guid.NewGuid():N}"[..20], insuranceType = "vehicle",
            startDate = "2026-01-01", expirationDate = "2026-12-31", premiumAmount = 1000, coverageAmount = 50000
        });
        var policyId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var renewal = await client.PostAsJsonAsync("/api/v1/renewals", new { policyId, status = "approved" });
        Assert.Equal(HttpStatusCode.Created, renewal.StatusCode);
        Assert.Equal("pending", (await renewal.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("status").GetString());

        var duplicate = await client.PostAsJsonAsync("/api/v1/renewals", new { policyId });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var renewals = await client.GetFromJsonAsync<JsonElement>($"/api/v1/renewals?policyId={policyId}");
        Assert.Equal(1, renewals.GetArrayLength());

        var unknown = await client.PostAsJsonAsync("/api/v1/renewals", new { policyId = 999999 });
        Assert.Equal(HttpStatusCode.NotFound, unknown.StatusCode);
    }

    [Fact]
    public async Task Policies_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await factory.CreateClient().GetAsync("/api/v1/policies");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}