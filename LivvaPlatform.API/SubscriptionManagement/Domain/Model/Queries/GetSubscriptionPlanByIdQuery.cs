namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a subscription plan by its identifier.
/// </summary>
public record GetSubscriptionPlanByIdQuery(long PlanId);
