using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Represents a vehicle insurance claim reported by a policyholder.
/// </summary>
/// <remarks>
///     A vehicle claim belongs to an existing insurance policy and the user
///     who reports the incident. Its status changes are recorded in the
///     associated ClaimStatusHistory collection.
///     Audit timestamps are managed by the persistence layer.
/// </remarks>
public partial class VehicleClaim
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected VehicleClaim()
    {
        ClaimNumber = string.Empty;
        Description = string.Empty;
        StatusHistories = new List<ClaimStatusHistory>();
    }

    /// <summary>
    ///     Registers a new vehicle insurance claim.
    /// </summary>
    /// <param name="command">The vehicle claim creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is invalid.</exception>
    public VehicleClaim(CreateVehicleClaimCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.ClaimNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Description);

        if (command.PolicyId <= 0 || command.UserId <= 0)
            throw new ArgumentException("Policy and user identifiers must be positive.", nameof(command));

        if (command.IncidentDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("The incident date cannot be in the future.", nameof(command));

        if (command.CompensationAmount is <= 0)
            throw new ArgumentException("The compensation amount must be positive.", nameof(command));

        PolicyId = command.PolicyId;
        UserId = command.UserId;
        ClaimNumber = command.ClaimNumber.Trim().ToUpperInvariant();
        IncidentDate = command.IncidentDate;
        Description = command.Description.Trim();
        CompensationAmount = command.CompensationAmount;
        Status = VehicleClaimStatus.REPORTED;

        StatusHistories = new List<ClaimStatusHistory>
        {
            new(Status, "Vehicle claim registered.")
        };
    }

    /// <summary>Gets the identifier of the vehicle claim.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the associated insurance policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the identifier of the user reporting the claim.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the unique claim number.</summary>
    public string ClaimNumber { get; private set; }

    /// <summary>Gets the date when the vehicle incident occurred.</summary>
    public DateOnly IncidentDate { get; private set; }

    /// <summary>Gets the description of the vehicle incident.</summary>
    public string Description { get; private set; }

    /// <summary>Gets the current status of the vehicle claim.</summary>
    public VehicleClaimStatus Status { get; private set; }

    /// <summary>Gets the compensation amount associated with the claim.</summary>
    public decimal? CompensationAmount { get; private set; }

    /// <summary>Gets the date when the claim was resolved.</summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>Gets the status history associated with this vehicle claim.</summary>
    public ICollection<ClaimStatusHistory> StatusHistories { get; private set; }

    /// <summary>
    ///     Changes the status of the vehicle claim and records the change.
    /// </summary>
    /// <param name="status">The new vehicle claim status.</param>
    /// <param name="comment">The reason for changing the status.</param>
    /// <exception cref="ArgumentException">Thrown when the comment is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the claim is already closed.</exception>
    public void ChangeStatus(VehicleClaimStatus status, string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);

        if (Status is VehicleClaimStatus.RESOLVED or VehicleClaimStatus.REJECTED)
            throw new InvalidOperationException("A closed vehicle claim cannot change its status.");

        Status = status;

        if (status is VehicleClaimStatus.RESOLVED or VehicleClaimStatus.REJECTED)
            ResolvedAt = DateTimeOffset.UtcNow;

        StatusHistories.Add(new ClaimStatusHistory(status, comment));
    }
}