namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of a subscription payment.
/// </summary>
public enum PaymentStatus
{
    /// <summary>The payment was initiated.</summary>
    PENDING,

    /// <summary>The payment provider approved the payment.</summary>
    APPROVED,

    /// <summary>The payment failed.</summary>
    FAILED
}
