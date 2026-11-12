namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

/// <summary>
///     Query to get user subscriptions, optionally filtered by user.
/// </summary>
public record GetUserSubscriptionsQuery(long? UserId);
