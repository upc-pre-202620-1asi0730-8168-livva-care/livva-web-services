namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a subscription payment by its identifier.
/// </summary>
public record GetSubscriptionPaymentByIdQuery(long SubscriptionPaymentId);
