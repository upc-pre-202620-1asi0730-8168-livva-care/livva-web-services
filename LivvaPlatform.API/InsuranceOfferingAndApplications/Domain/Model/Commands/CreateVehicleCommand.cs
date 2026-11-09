namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to register a vehicle.
/// </summary>
public record CreateVehicleCommand(long UserId, string LicensePlate, string Brand, string Model, int ManufactureYear, decimal EstimatedValue);
