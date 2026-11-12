using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Represents a status change in a vehicle insurance claim.
/// </summary>
/// <remarks>
///     Each status history entry belongs to a vehicle claim and records
///     the new status, the comment explaining the change and its timestamp.
/// </remarks>
public class ClaimStatusHistory
{
    /// <summary>
    ///     Protected parameterless constructor for Entity Framework Core.
    /// </summary>
    protected ClaimStatusHistory()
    {
        Comment = string.Empty;
    }

    /// <summary>
    ///     Creates a vehicle claim status history entry.
    /// </summary>
    /// <param name="status">The status assigned to the vehicle claim.</param>
    /// <param name="comment">The comment explaining the status change.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the comment is null, empty or whitespace.
    /// </exception>
    public ClaimStatusHistory(VehicleClaimStatus status, string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);

        Status = status;
        Comment = comment.Trim();
        ChangedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Gets the identifier of the status history entry.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the associated vehicle claim.</summary>
    public long ClaimId { get; private set; }

    /// <summary>Gets the status assigned to the vehicle claim.</summary>
    public VehicleClaimStatus Status { get; private set; }

    /// <summary>Gets the comment explaining the status change.</summary>
    public string Comment { get; private set; }

    /// <summary>Gets the date and time when the status was changed.</summary>
    public DateTimeOffset ChangedAt { get; private set; }
} 