using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Represents an application submitted by a user for an insurance product.
/// </summary>
/// <remarks>
///     A new application always starts as <see cref="RequestStatus.PENDING" />. Its vehicle or life details are stored
///     in <see cref="VehicleInsuranceApplication" /> or <see cref="LifeInsuranceApplication" />.
/// </remarks>
public partial class InsuranceApplication
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected InsuranceApplication()
    {
    }

    /// <summary>
    ///     Submits a new application from a creation command.
    /// </summary>
    /// <param name="command">The create insurance application command.</param>
    /// <exception cref="ArgumentException">Thrown when the requested coverage is not positive.</exception>
    public InsuranceApplication(CreateInsuranceApplicationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.RequestedCoverage <= 0)
            throw new ArgumentException("The requested coverage must be positive.", nameof(command));

        UserId = command.UserId;
        ProductId = command.ProductId;
        RequestedCoverage = command.RequestedCoverage;
        Status = RequestStatus.PENDING;
    }

    /// <summary>Gets the identifier of the application.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the applicant user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the identifier of the selected insurance product.</summary>
    public long ProductId { get; private set; }

    /// <summary>Gets the requested coverage amount.</summary>
    public decimal RequestedCoverage { get; private set; }

    /// <summary>Gets the status of the application.</summary>
    public RequestStatus Status { get; private set; }

    /// <summary>Starts the evaluation of a pending application.</summary>
    public void StartEvaluation() => ChangeStatus(RequestStatus.PENDING, RequestStatus.UNDER_REVIEW);

    /// <summary>Approves an application under review.</summary>
    public void Approve() => ChangeStatus(RequestStatus.UNDER_REVIEW, RequestStatus.APPROVED);

    /// <summary>Rejects an application under review.</summary>
    public void Reject() => ChangeStatus(RequestStatus.UNDER_REVIEW, RequestStatus.REJECTED);

    /// <summary>Cancels a pending or under review application.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the application was already resolved.</exception>
    public void Cancel()
    {
        if (Status is not (RequestStatus.PENDING or RequestStatus.UNDER_REVIEW))
            throw new InvalidOperationException("Only pending or under review applications can be cancelled.");
        Status = RequestStatus.CANCELLED;
    }

    private void ChangeStatus(RequestStatus expected, RequestStatus next)
    {
        if (Status != expected)
            throw new InvalidOperationException($"The application must be {expected} to become {next}.");
        Status = next;
    }
}