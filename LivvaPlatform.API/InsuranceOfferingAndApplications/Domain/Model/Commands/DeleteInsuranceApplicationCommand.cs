namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to delete an insurance application.
/// </summary>
public record DeleteInsuranceApplicationCommand(long ApplicationId);
