using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a payment of a user subscription processed by a payment provider.
/// </summary>
public partial class SubscriptionPayment
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected SubscriptionPayment()
    {
        Provider = string.Empty;
        Currency = string.Empty;
    }

    /// <summary>
    ///     Registers a payment from a creation command.
    /// </summary>
    /// <param name="command">The create subscription payment command.</param>
    /// <exception cref="ArgumentException">Thrown when a value is not valid.</exception>
    public SubscriptionPayment(CreateSubscriptionPaymentCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Currency);
        if (command.Amount <= 0) throw new ArgumentException("The amount must be positive.", nameof(command));

        UserSubscriptionId = command.UserSubscriptionId;
        UserId = command.UserId;
        PlanId = command.PlanId;
        Provider = command.Provider.Trim();
        ProviderPaymentId = string.IsNullOrWhiteSpace(command.ProviderPaymentId) ? null : command.ProviderPaymentId.Trim();
        Amount = command.Amount;
        Currency = command.Currency.Trim().ToUpperInvariant();
        Status = command.Status;
        PaidAt = command.Status == PaymentStatus.APPROVED ? command.PaidAt ?? DateTimeOffset.UtcNow : command.PaidAt;
    }

    /// <summary>Gets the identifier of the payment.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the subscription.</summary>
    public long UserSubscriptionId { get; private set; }

    /// <summary>Gets the identifier of the user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the identifier of the plan.</summary>
    public long PlanId { get; private set; }

    /// <summary>Gets the payment provider (for example, mercado-pago-sandbox).</summary>
    public string Provider { get; private set; }

    /// <summary>Gets the identifier of the payment in the provider.</summary>
    public string? ProviderPaymentId { get; private set; }

    /// <summary>Gets the amount.</summary>
    public decimal Amount { get; private set; }

    /// <summary>Gets the ISO currency code (for example, PEN).</summary>
    public string Currency { get; private set; }

    /// <summary>Gets the status of the payment.</summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>Gets the timestamp when the payment was approved.</summary>
    public DateTimeOffset? PaidAt { get; private set; }

    /// <summary>Marks the payment as initiated.</summary>
    public void Initiate() => Status = PaymentStatus.PENDING;

    /// <summary>Confirms the payment.</summary>
    public void Confirm()
    {
        Status = PaymentStatus.APPROVED;
        PaidAt ??= DateTimeOffset.UtcNow;
    }

    /// <summary>Marks the payment as failed.</summary>
    public void Fail() => Status = PaymentStatus.FAILED;
}