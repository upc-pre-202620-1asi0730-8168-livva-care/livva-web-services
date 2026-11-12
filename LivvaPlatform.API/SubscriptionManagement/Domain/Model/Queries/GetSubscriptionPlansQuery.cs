namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

/// <summary>
///     Query to get subscription plans, optionally only the active ones.
/// </summary>
public record GetSubscriptionPlansQuery(bool? Active);
