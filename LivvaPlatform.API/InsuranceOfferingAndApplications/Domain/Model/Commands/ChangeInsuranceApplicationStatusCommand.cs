using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to change the status of an insurance application.
/// </summary>
/// <param name="ApplicationId">The identifier of the insurance application.</param>
/// <param name="Status">The new status.</param>
public record ChangeInsuranceApplicationStatusCommand(long ApplicationId, RequestStatus Status);