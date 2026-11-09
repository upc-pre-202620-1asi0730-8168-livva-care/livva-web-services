namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to submit an insurance application for a product.
/// </summary>
public record CreateInsuranceApplicationCommand(long UserId, long ProductId, decimal RequestedCoverage);
