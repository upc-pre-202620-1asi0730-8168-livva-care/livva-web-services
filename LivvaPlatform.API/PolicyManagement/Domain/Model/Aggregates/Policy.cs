using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents an insurance policy issued by an insurance company for a Livva user.
/// </summary>
/// <remarks>The policy number is unique. Coverages, documents and renewals reference the policy by its identifier.</remarks>
public partial class Policy
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Policy()
    {
        PolicyNumber = string.Empty;
    }

    /// <summary>
    ///     Registers an issued policy. The policy starts as active.
    /// </summary>
    /// <param name="command">The create policy command.</param>
    /// <exception cref="ArgumentException">Thrown when a value is not valid.</exception>
    public Policy(CreatePolicyCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.PolicyNumber);
        if (command.ExpirationDate <= command.StartDate)
            throw new ArgumentException("The expiration date must be after the start date.", nameof(command));
        if (command.PremiumAmount <= 0 || command.CoverageAmount <= 0)
            throw new ArgumentException("The premium and coverage amounts must be positive.", nameof(command));

        UserId = command.UserId;
        ApplicationId = command.ApplicationId;
        PolicyNumber = command.PolicyNumber.Trim().ToUpperInvariant();
        InsuranceType = command.InsuranceType;
        StartDate = command.StartDate;
        ExpirationDate = command.ExpirationDate;
        PremiumAmount = command.PremiumAmount;
        CoverageAmount = command.CoverageAmount;
        Status = PolicyStatus.ACTIVE;
    }

    /// <summary>Gets the identifier of the policy.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the policyholder user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the identifier of the insurance application that generated the policy.</summary>
    public long ApplicationId { get; private set; }

    /// <summary>Gets the unique policy number.</summary>
    public string PolicyNumber { get; private set; }

    /// <summary>Gets the type of insurance.</summary>
    public InsuranceType InsuranceType { get; private set; }

    /// <summary>Gets the start date of the coverage.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>Gets the expiration date of the coverage.</summary>
    public DateOnly ExpirationDate { get; private set; }

    /// <summary>Gets the premium amount.</summary>
    public decimal PremiumAmount { get; private set; }

    /// <summary>Gets the coverage amount.</summary>
    public decimal CoverageAmount { get; private set; }

    /// <summary>Gets the status of the policy.</summary>
    public PolicyStatus Status { get; private set; }

    /// <summary>Gets a value indicating whether a renewal can be requested for the policy.</summary>
    public bool CanBeRenewed => Status is PolicyStatus.ACTIVE or PolicyStatus.EXPIRED;

    /// <summary>Activates the policy.</summary>
    public void Activate() => Status = PolicyStatus.ACTIVE;

    /// <summary>Marks the policy as pending renewal because it is close to its expiration date.</summary>
    public void MarkAsExpiring() => Status = PolicyStatus.PENDING_RENEWAL;

    /// <summary>Marks the policy as expired.</summary>
    public void Expire() => Status = PolicyStatus.EXPIRED;

    /// <summary>
    ///     Creates a renewal request for the policy.
    /// </summary>
    /// <returns>The renewal request.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the policy cannot be renewed.</exception>
    public Renewal RequestRenewal()
    {
        if (!CanBeRenewed) throw new InvalidOperationException("The policy cannot be renewed in its current status.");
        return new Renewal(new CreateRenewalCommand(Id));
    }
}