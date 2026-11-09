using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Data required to link an insurance application with a vehicle.</summary>
public record CreateVehicleInsuranceApplicationResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredApplicationId")] long ApplicationId,
    [Range(1, long.MaxValue, ErrorMessage = "RequiredVehicleId")] long VehicleId);
