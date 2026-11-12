namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>A subscription payment returned by the API. The status is <c>pending</c>, <c>approved</c> or <c>failed</c>.</summary>
public record SubscriptionPaymentResource(long Id, long UserSubscriptionId, long UserId, long PlanId, string Provider, string? ProviderPaymentId, decimal Amount, string Currency, string Status, DateTimeOffset? PaidAt, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
