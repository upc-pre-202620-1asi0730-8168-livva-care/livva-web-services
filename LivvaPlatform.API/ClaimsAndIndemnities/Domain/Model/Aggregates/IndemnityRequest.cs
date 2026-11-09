using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Represents a life insurance indemnity request submitted by a beneficiary.
/// </summary>
/// <remarks>
///     An indemnity request is associated with an existing insurance policy
///     and its beneficiary. Changes to its status are recorded in the
///     associated IndemnityStatusHistory collection.
///     Audit timestamps are managed by the persistence layer.
/// </remarks>
public partial class IndemnityRequest
{
    /// <summary>
    ///     Protected parameterless constructor for Entity Framework Core.
    /// </summary>
    protected IndemnityRequest()
    {
        Description = string.Empty;
        StatusHistories = new List<IndemnityStatusHistory>();
    }

    /// <summary>
    ///     Creates a new life insurance indemnity request.
    /// </summary>
    /// <param name="command">The indemnity request creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is invalid.</exception>
    public IndemnityRequest(CreateIndemnityRequestCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Description);

        if (command.PolicyId <= 0 || command.BeneficiaryId <= 0)
            throw new ArgumentException(
                "Policy and beneficiary identifiers must be positive.",
                nameof(command));

        if (command.RequestDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException(
                "The request date cannot be in the future.",
                nameof(command));

        if (command.IndemnityAmount is <= 0)
            throw new ArgumentException(
                "The indemnity amount must be positive.",
                nameof(command));

        PolicyId = command.PolicyId;
        BeneficiaryId = command.BeneficiaryId;
        RequestDate = command.RequestDate;
        Description = command.Description.Trim();
        IndemnityAmount = command.IndemnityAmount;
        Status = IndemnityRequestStatus.SUBMITTED;

        StatusHistories = new List<IndemnityStatusHistory>
        {
            new(Status, "Indemnity request submitted.")
        };
    }

    /// <summary>Gets the identifier of the indemnity request.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the associated insurance policy.</summary>
    public long PolicyId { get; private set; }

    /// <summary>Gets the identifier of the beneficiary submitting the request.</summary>
    public long BeneficiaryId { get; private set; }

    /// <summary>Gets the date when the indemnity request was submitted.</summary>
    public DateOnly RequestDate { get; private set; }

    /// <summary>Gets the description of the indemnity request.</summary>
    public string Description { get; private set; }

    /// <summary>Gets the current status of the indemnity request.</summary>
    public IndemnityRequestStatus Status { get; private set; }

    /// <summary>Gets the requested indemnity amount, if specified.</summary>
    public decimal? IndemnityAmount { get; private set; }

    /// <summary>Gets the date when the indemnity request was resolved.</summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>Gets the status history associated with this indemnity request.</summary>
    public ICollection<IndemnityStatusHistory> StatusHistories { get; private set; }

    /// <summary>
    ///     Changes the status of the indemnity request and records the change.
    /// </summary>
    /// <param name="status">The new indemnity request status.</param>
    /// <param name="comment">The reason for changing the status.</param>
    /// <exception cref="ArgumentException">Thrown when the comment is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the request is already closed.</exception>
    public void ChangeStatus(IndemnityRequestStatus status, string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);

        if (Status is IndemnityRequestStatus.PAID or IndemnityRequestStatus.REJECTED)
            throw new InvalidOperationException(
                "A closed indemnity request cannot change its status.");

        Status = status;

        if (status is IndemnityRequestStatus.PAID or IndemnityRequestStatus.REJECTED)
            ResolvedAt = DateTimeOffset.UtcNow;

        StatusHistories.Add(new IndemnityStatusHistory(status, comment));
    }
}
