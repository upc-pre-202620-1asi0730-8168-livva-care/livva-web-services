namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

/// <summary>
///     Billing cycles of a Livva subscription.
/// </summary>
public enum BillingCycle
{
    /// <summary>Billed every month.</summary>
    MONTHLY,

    /// <summary>Billed every year.</summary>
    ANNUAL
}
