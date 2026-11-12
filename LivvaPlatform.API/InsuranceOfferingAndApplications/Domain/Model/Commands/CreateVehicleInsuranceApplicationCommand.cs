namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to link an insurance application with a vehicle.
/// </summary>
public record CreateVehicleInsuranceApplicationCommand(long ApplicationId, long VehicleId);
