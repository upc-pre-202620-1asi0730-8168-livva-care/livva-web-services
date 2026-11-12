namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to mark an insurance application as a life insurance application.
/// </summary>
public record CreateLifeInsuranceApplicationCommand(long ApplicationId);
