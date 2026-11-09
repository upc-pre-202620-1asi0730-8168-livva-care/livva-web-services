namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>The vehicle details of an insurance application.</summary>
public record VehicleInsuranceApplicationResource(long Id, long ApplicationId, long VehicleId);
