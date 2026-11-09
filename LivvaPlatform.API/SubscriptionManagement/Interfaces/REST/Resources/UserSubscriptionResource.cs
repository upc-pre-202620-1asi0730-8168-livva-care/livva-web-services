namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>A user subscription returned by the API. The billing cycle is <c>monthly</c> or <c>annual</c>; the status is <c>pending</c>, <c>active</c>, <c>cancelled</c> or <c>expired</c>.</summary>
public record UserSubscriptionResource(long Id, long UserId, long PlanId, string BillingCycle, string Status, DateTimeOffset StartDate, DateTimeOffset NextBillingDate, DateTimeOffset? CancellationRequestedAt, DateTimeOffset? CancelledAt, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
