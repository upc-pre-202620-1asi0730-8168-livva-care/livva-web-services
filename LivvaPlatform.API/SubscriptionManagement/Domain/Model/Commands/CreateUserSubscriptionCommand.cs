using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;

/// <summary>
///     Command to subscribe a user to a plan after its payment was approved.
/// </summary>
public record CreateUserSubscriptionCommand(long UserId, long PlanId, BillingCycle BillingCycle);
