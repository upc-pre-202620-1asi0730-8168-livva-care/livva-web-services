namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;

/// <summary>
///     Command to delete a user subscription and its payments.
/// </summary>
public record DeleteUserSubscriptionCommand(long UserSubscriptionId);
