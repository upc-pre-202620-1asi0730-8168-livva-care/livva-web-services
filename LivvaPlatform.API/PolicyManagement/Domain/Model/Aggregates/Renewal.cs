using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a request to renew a policy.
/// </summary>
/// <remarks>The proposed expiration date, proposed premium and resolution date are set when the insurer resolves it.</remarks>
public partial class Renewal
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Renewal()
    {
    }

    /// <summary>
    ///     Requests the renewal of a policy. The renewal starts as pending.
    /// </summary>
    /// <param name="command">The create renewal command.</param>
    public Renewal(CreateRenewalCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        PolicyId = command.PolicyId;
        RequestedAt = DateTimeOffset.UtcNow;
        Status = RenewalStatus.PENDING;
    }

    /// <summary>Gets the identifier of the renewal.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the timestamp when the renewal was requested.</summary>
    public DateTimeOffset RequestedAt { get; private set; }

    /// <summary>Gets the expiration date proposed by the insurer.</summary>
    public DateOnly? ProposedExpirationDate { get; private set; }

    /// <summary>Gets the premium proposed by the insurer.</summary>
    public decimal? ProposedPremium { get; private set; }

    /// <summary>Gets the status of the renewal.</summary>
    public RenewalStatus Status { get; private set; }

    /// <summary>Gets the timestamp when the renewal was resolved.</summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>Gets a value indicating whether the renewal is still open.</summary>
    public bool IsOpen => Status is RenewalStatus.PENDING or RenewalStatus.UNDER_REVIEW;

    /// <summary>Starts the review of a pending renewal.</summary>
    public void StartReview()
    {
        if (Status != RenewalStatus.PENDING) throw new InvalidOperationException("Only pending renewals can be reviewed.");
        Status = RenewalStatus.UNDER_REVIEW;
    }

    /// <summary>
    ///     Approves the renewal with the terms proposed by the insurer.
    /// </summary>
    /// <param name="proposedExpirationDate">The new expiration date.</param>
    /// <param name="proposedPremium">The new premium.</param>
    public void Approve(DateOnly proposedExpirationDate, decimal proposedPremium)
    {
        if (!IsOpen) throw new InvalidOperationException("The renewal was already resolved.");
        ProposedExpirationDate = proposedExpirationDate;
        ProposedPremium = proposedPremium;
        Status = RenewalStatus.APPROVED;
        ResolvedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Rejects the renewal.</summary>
    public void Reject()
    {
        if (!IsOpen) throw new InvalidOperationException("The renewal was already resolved.");
        Status = RenewalStatus.REJECTED;
        ResolvedAt = DateTimeOffset.UtcNow;
    }
}