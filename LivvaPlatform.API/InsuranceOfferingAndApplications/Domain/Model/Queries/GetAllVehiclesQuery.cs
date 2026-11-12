namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;

/// <summary>
///     Query to get vehicles, optionally filtered by owner.
/// </summary>
public record GetAllVehiclesQuery(long? UserId);
