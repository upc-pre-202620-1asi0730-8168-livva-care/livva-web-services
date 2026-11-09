namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get life insurance applications, optionally filtered by application.
/// </summary>
public record GetAllLifeInsuranceApplicationsQuery(long? ApplicationId);
