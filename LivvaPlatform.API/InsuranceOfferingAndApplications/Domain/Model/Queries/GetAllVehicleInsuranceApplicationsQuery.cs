namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get vehicle insurance applications, optionally filtered by application.
/// </summary>
public record GetAllVehicleInsuranceApplicationsQuery(long? ApplicationId);
