using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents the subscription of a user to a Livva plan.
/// </summary>
public partial class UserSubscription
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected UserSubscription()
    {
    }

    /// <summary>
    ///     Creates an active subscription. The next billing date is one month or one year after the start date.
    /// </summary>
    /// <param name="command">The create user subscription command.</param>
    public UserSubscription(CreateUserSubscriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        UserId = command.UserId;
        PlanId = command.PlanId;
        BillingCycle = command.BillingCycle;
        StartDate = DateTimeOffset.UtcNow;
        NextBillingDate = BillingCycle == BillingCycle.ANNUAL ? StartDate.AddYears(1) : StartDate.AddMonths(1);
        Status = SubscriptionStatus.ACTIVE;
    }

    /// <summary>Gets the identifier of the subscription.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the subscribed user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the identifier of the plan.</summary>
    public long PlanId { get; private set; }

    /// <summary>Gets the billing cycle.</summary>
    public BillingCycle BillingCycle { get; private set; }

    /// <summary>Gets the status of the subscription.</summary>
    public SubscriptionStatus Status { get; private set; }

    /// <summary>Gets the start date.</summary>
    public DateTimeOffset StartDate { get; private set; }

    /// <summary>Gets the next billing date.</summary>
    public DateTimeOffset NextBillingDate { get; private set; }

    /// <summary>Gets the timestamp when the cancellation was requested.</summary>
    public DateTimeOffset? CancellationRequestedAt { get; private set; }

    /// <summary>Gets the timestamp when the subscription was cancelled.</summary>
    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>Activates the subscription.</summary>
    public void Activate() => Status = SubscriptionStatus.ACTIVE;

    /// <summary>
    ///     Cancels an active subscription.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the subscription is not active.</exception>
    public void Cancel()
    {
        if (Status != SubscriptionStatus.ACTIVE) throw new InvalidOperationException("Only active subscriptions can be cancelled.");
        var now = DateTimeOffset.UtcNow;
        CancellationRequestedAt = now;
        CancelledAt = now;
        Status = SubscriptionStatus.CANCELLED;
    }

    /// <summary>Marks the subscription as expired.</summary>
    public void Expire() => Status = SubscriptionStatus.EXPIRED;
}