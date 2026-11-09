using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Data required to mark an insurance application as a life insurance application.</summary>
public record CreateLifeInsuranceApplicationResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredApplicationId")] long ApplicationId);
