using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Represents a status change in a life insurance indemnity request.
/// </summary>
/// <remarks>
///     Each history entry belongs to an indemnity request and records
///     the assigned status, the comment explaining the change and
///     the date and time when the change occurred.
/// </remarks>
public class IndemnityStatusHistory
{
    /// <summary>
    ///     Protected parameterless constructor for Entity Framework Core.
    /// </summary>
    protected IndemnityStatusHistory()
    {
        Comment = string.Empty;
    }

    /// <summary>
    ///     Creates a new indemnity request status history entry.
    /// </summary>
    /// <param name="status">The status assigned to the indemnity request.</param>
    /// <param name="comment">The comment explaining the status change.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the comment is null, empty or whitespace.
    /// </exception>
    public IndemnityStatusHistory(
        IndemnityRequestStatus status,
        string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);

        Status = status;
        Comment = comment.Trim();
        ChangedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Gets the identifier of the status history entry.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the associated indemnity request.</summary>
    public long IndemnityRequestId { get; private set; }

    /// <summary>Gets the status assigned to the indemnity request.</summary>
    public IndemnityRequestStatus Status { get; private set; }

    /// <summary>Gets the comment explaining the status change.</summary>
    public string Comment { get; private set; }

    /// <summary>Gets the date and time when the status was changed.</summary>
    public DateTimeOffset ChangedAt { get; private set; }
}