using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Data required to register a vehicle.</summary>
public record CreateVehicleResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Required(ErrorMessage = "RequiredLicensePlate")] [StringLength(20)] string LicensePlate,
    [Required(ErrorMessage = "RequiredBrand")] [StringLength(100)] string Brand,
    [Required(ErrorMessage = "RequiredModel")] [StringLength(100)] string Model,
    [Range(1900, 2100, ErrorMessage = "InvalidManufactureYear")] int ManufactureYear,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal EstimatedValue);
