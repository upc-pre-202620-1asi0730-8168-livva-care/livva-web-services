namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

/// <summary>
///     Query to get subscription payments, optionally filtered by user and subscription.
/// </summary>
public record GetSubscriptionPaymentsQuery(long? UserId, long? UserSubscriptionId);
