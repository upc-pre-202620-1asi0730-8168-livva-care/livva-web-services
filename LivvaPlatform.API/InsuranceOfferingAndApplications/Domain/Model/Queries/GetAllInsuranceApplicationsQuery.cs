namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get insurance applications, optionally filtered by applicant.
/// </summary>
public record GetAllInsuranceApplicationsQuery(long? UserId);
