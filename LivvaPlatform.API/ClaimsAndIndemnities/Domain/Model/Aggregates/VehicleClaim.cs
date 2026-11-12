using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Vehicle claim aggregate root. It represents an incident reported by the insured
///     for a vehicle insurance policy, together with its status history.
/// </summary>
public partial class VehicleClaim
{
    /// <summary>Constructor used by Entity Framework.</summary>
    protected VehicleClaim()
    {
        IncidentLocation = string.Empty;
        Description = string.Empty;
        StatusHistories = new List<ClaimStatusHistory>();
    }

    /// <summary>
    ///     Registers a new vehicle claim in <c>reported</c> status and records its first status history entry.
    /// </summary>
    /// <param name="command">The command with the claim data.</param>
    public VehicleClaim(CreateVehicleClaimCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.IncidentLocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Description);

        if (command.PolicyId <= 0 || command.UserId <= 0)
            throw new ArgumentException("Policy and user identifiers must be positive.", nameof(command));

        if (command.IncidentDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("The incident date cannot be in the future.", nameof(command));

        if (command.EstimatedDamageAmount <= 0)
            throw new ArgumentException("The estimated damage amount must be positive.", nameof(command));

        PolicyId = command.PolicyId;
        UserId = command.UserId;
        IncidentDate = command.IncidentDate;
        IncidentLocation = command.IncidentLocation.Trim();
        Description = command.Description.Trim();
        EstimatedDamageAmount = command.EstimatedDamageAmount;
        Status = VehicleClaimStatus.REPORTED;
        ReportedAt = DateTimeOffset.UtcNow;

        StatusHistories = new List<ClaimStatusHistory>
        {
            new(Status, "Siniestro registrado por el asegurado.")
        };
    }

    /// <summary>The identifier of the claim.</summary>
    public long Id { get; private set; }

    /// <summary>The identifier of the vehicle insurance policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>The identifier of the insured user.</summary>
    public long UserId { get; private set; }

    /// <summary>The date of the incident.</summary>
    public DateOnly IncidentDate { get; private set; }

    /// <summary>The place where the incident happened.</summary>
    public string IncidentLocation { get; private set; }

    /// <summary>The description of the incident.</summary>
    public string Description { get; private set; }

    /// <summary>The estimated amount of the damage.</summary>
    public decimal EstimatedDamageAmount { get; private set; }

    /// <summary>The current status of the claim.</summary>
    public VehicleClaimStatus Status { get; private set; }

    /// <summary>The compensation amount, once defined by the insurance company.</summary>
    public decimal? CompensationAmount { get; private set; }

    /// <summary>When the claim was reported.</summary>
    public DateTimeOffset ReportedAt { get; private set; }

    /// <summary>When the claim was closed (resolved, rejected or cancelled).</summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>The status history of the claim.</summary>
    public ICollection<ClaimStatusHistory> StatusHistories { get; private set; }

    /// <summary>Indicates whether the claim is closed and cannot change its status anymore.</summary>
    public bool IsClosed =>
        Status is VehicleClaimStatus.RESOLVED or VehicleClaimStatus.REJECTED or VehicleClaimStatus.CANCELLED;

    /// <summary>
    ///     Changes the status of the claim and records the change in its history.
    ///     A claim can only be cancelled while it is <c>reported</c> or <c>under_review</c>.
    /// </summary>
    /// <param name="status">The new status.</param>
    /// <param name="comment">An optional comment. A default comment is used when it is empty.</param>
    /// <exception cref="InvalidOperationException">When the claim is closed or the change is not allowed.</exception>
    public void ChangeStatus(VehicleClaimStatus status, string? comment = null)
    {
        if (IsClosed)
            throw new InvalidOperationException("A closed vehicle claim cannot change its status.");

        if (status == Status)
            throw new InvalidOperationException("The vehicle claim already has this status.");

        if (status is VehicleClaimStatus.CANCELLED &&
            Status is not (VehicleClaimStatus.REPORTED or VehicleClaimStatus.UNDER_REVIEW))
            throw new InvalidOperationException("Only reported or under review claims can be cancelled.");

        Status = status;

        if (IsClosed)
            ResolvedAt = DateTimeOffset.UtcNow;

        var historyComment = string.IsNullOrWhiteSpace(comment)
            ? status is VehicleClaimStatus.CANCELLED
                ? "Siniestro cancelado por el asegurado."
                : $"Estado actualizado a {status.ToString().ToLowerInvariant()}."
            : comment;

        StatusHistories.Add(new ClaimStatusHistory(status, historyComment));
    }
}
