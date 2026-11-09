namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get beneficiaries, optionally filtered by policy.
/// </summary>
public record GetAllBeneficiariesQuery(long? PolicyId);
