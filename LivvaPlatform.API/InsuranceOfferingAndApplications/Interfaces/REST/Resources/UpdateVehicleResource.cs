using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Fields of a vehicle that can be partially updated. Omitted fields keep their current value.</summary>
public record UpdateVehicleResource(
    [StringLength(20)] string? LicensePlate,
    [StringLength(100)] string? Brand,
    [StringLength(100)] string? Model,
    [Range(1900, 2100, ErrorMessage = "InvalidManufactureYear")] int? ManufactureYear,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal? EstimatedValue);
