namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to partially update a vehicle. Null values keep the current value.
/// </summary>
public record UpdateVehicleCommand(long VehicleId, string? LicensePlate, string? Brand, string? Model, int? ManufactureYear, decimal? EstimatedValue);
