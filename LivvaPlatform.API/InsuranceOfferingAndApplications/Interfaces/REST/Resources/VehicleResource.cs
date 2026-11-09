namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>A vehicle returned by the API.</summary>
public record VehicleResource(long Id, long UserId, string LicensePlate, string Brand, string Model, int ManufactureYear, decimal EstimatedValue, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
