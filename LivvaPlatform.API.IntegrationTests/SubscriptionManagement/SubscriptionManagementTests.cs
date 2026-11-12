using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LivvaPlatform.API.IntegrationTests.SubscriptionManagement;

/// <summary>
///     Integration tests for the Subscription Management endpoints.
/// </summary>
public class SubscriptionManagementTests(LivvaWebApplicationFactory factory) : IClassFixture<LivvaWebApplicationFactory>
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
    public async Task Plans_ShouldReturnSeededActivePlansWithBenefits()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var plans = await client.GetFromJsonAsync<JsonElement>("/api/v1/subscription-plans?active=true");

        Assert.Equal(2, plans.GetArrayLength());
        Assert.Equal(4, plans[0].GetProperty("benefits").GetArrayLength());
        Assert.Equal(19, plans[0].GetProperty("monthlyPrice").GetDecimal());
    }

    [Fact]
    public async Task SubscriptionFlowAsUsedByWebApplication_ShouldWork()
    {
        var (client, userId) = await AuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/user-subscriptions", new { userId, planId = 2, billingCycle = "annual" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var subscription = await create.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("active", subscription.GetProperty("status").GetString());
        Assert.Equal("annual", subscription.GetProperty("billingCycle").GetString());
        var subscriptionId = subscription.GetProperty("id").GetInt64();

        var duplicate = await client.PostAsJsonAsync("/api/v1/user-subscriptions", new { userId, planId = 1, billingCycle = "monthly" });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var payment = await client.PostAsJsonAsync("/api/v1/subscription-payments", new
        {
            userSubscriptionId = subscriptionId, userId, planId = 2, provider = "mercado-pago-sandbox",
            providerPaymentId = "PAY01TEST", amount = 390, currency = "PEN", status = "approved"
        });
        Assert.Equal(HttpStatusCode.Created, payment.StatusCode);

        var payments = await client.GetFromJsonAsync<JsonElement>($"/api/v1/subscription-payments?userId={userId}");
        Assert.Equal(1, payments.GetArrayLength());
        Assert.Equal("approved", payments[0].GetProperty("status").GetString());

        var mine = await client.GetFromJsonAsync<JsonElement>($"/api/v1/user-subscriptions?userId={userId}");
        Assert.Equal(1, mine.GetArrayLength());

        var cancel = await client.PatchAsJsonAsync($"/api/v1/user-subscriptions/{subscriptionId}", new { status = "cancelled" });
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        var cancelled = await cancel.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("cancelled", cancelled.GetProperty("status").GetString());
        Assert.NotEqual(JsonValueKind.Null, cancelled.GetProperty("cancelledAt").ValueKind);

        var cancelAgain = await client.PatchAsJsonAsync($"/api/v1/user-subscriptions/{subscriptionId}", new { status = "cancelled" });
        Assert.Equal(HttpStatusCode.Conflict, cancelAgain.StatusCode);
    }

    [Fact]
    public async Task DeleteSubscription_ShouldRemoveItsPayments()
    {
        var (client, userId) = await AuthenticatedClientAsync();
        var create = await client.PostAsJsonAsync("/api/v1/user-subscriptions", new { userId, planId = 1, billingCycle = "monthly" });
        var subscriptionId = (await create.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        await client.PostAsJsonAsync("/api/v1/subscription-payments", new
        {
            userSubscriptionId = subscriptionId, userId, planId = 1, provider = "mercado-pago-sandbox", amount = 19,
            currency = "PEN", status = "approved"
        });

        var delete = await client.DeleteAsync($"/api/v1/user-subscriptions/{subscriptionId}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        var payments = await client.GetFromJsonAsync<JsonElement>($"/api/v1/subscription-payments?userSubscriptionId={subscriptionId}");
        Assert.Equal(0, payments.GetArrayLength());
    }

    [Fact]
    public async Task PaymentOrders_WithoutAccessToken_ShouldReturnServerError()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/payment-orders", new { type = "online" });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task InvalidBillingCycle_ShouldReturnBadRequest()
    {
        var (client, userId) = await AuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/user-subscriptions", new { userId, planId = 1, billingCycle = "weekly" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}