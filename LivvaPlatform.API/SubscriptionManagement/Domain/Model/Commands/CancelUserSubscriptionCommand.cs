namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;

/// <summary>
///     Command to cancel a user subscription.
/// </summary>
public record CancelUserSubscriptionCommand(long UserSubscriptionId);
