using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;

/// <summary>
///     Command to register a payment of a user subscription.
/// </summary>
public record CreateSubscriptionPaymentCommand(long UserSubscriptionId, long UserId, long PlanId, string Provider, string? ProviderPaymentId, decimal Amount, string Currency, PaymentStatus Status, DateTimeOffset? PaidAt);
