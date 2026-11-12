namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of a user subscription.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>The subscription waits for payment confirmation.</summary>
    PENDING,

    /// <summary>The subscription is active.</summary>
    ACTIVE,

    /// <summary>The subscription was cancelled.</summary>
    CANCELLED,

    /// <summary>The subscription expired.</summary>
    EXPIRED
}
