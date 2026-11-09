using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.NotificationManagement;

/// <summary>
///     Integration tests for the Notification Management endpoints.
/// </summary>
public class NotificationManagementTests(LivvaWebApplicationFactory factory) : IClassFixture<LivvaWebApplicationFactory>
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
    public async Task ExpirationNoticeFlowAsUsedByWebApplication_ShouldWork()
    {
        var (client, userId) = await AuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/notifications", new
        {
            userId, type = "policy_expiration", policyId = 1, policyNumber = "LIV-VEH-0001",
            expirationDate = "2027-01-14", isRead = true
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var notification = await create.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(notification.GetProperty("isRead").GetBoolean());
        var notificationId = notification.GetProperty("id").GetInt64();

        var duplicate = await client.PostAsJsonAsync("/api/v1/notifications",
            new { userId, type = "policy_expiration", policyId = 1, policyNumber = "LIV-VEH-0001" });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var mine = await client.GetFromJsonAsync<JsonElement>($"/api/v1/notifications?userId={userId}");
        Assert.Equal(1, mine.GetArrayLength());

        var read = await client.PatchAsJsonAsync($"/api/v1/notifications/{notificationId}",
            new { isRead = true, readAt = DateTimeOffset.UtcNow });
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        var updated = await read.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(updated.GetProperty("isRead").GetBoolean());
        Assert.NotEqual(JsonValueKind.Null, updated.GetProperty("readAt").ValueKind);
    }

    [Fact]
    public async Task Create_ForUnknownPolicy_ShouldReturnNotFound()
    {
        var (client, userId) = await AuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/notifications",
            new { userId, type = "policy_expiration", policyId = 999999 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_WithUnsupportedUpdate_ShouldReturnBadRequest()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var create = await client.PostAsJsonAsync("/api/v1/notifications", new { userId, type = "general" });
        var notificationId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        var response = await client.PatchAsJsonAsync($"/api/v1/notifications/{notificationId}", new { isRead = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}