using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Data required to submit an insurance application. New applications always start as <c>pending</c>.</summary>
public record CreateInsuranceApplicationResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "RequiredProductId")] long ProductId,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal RequestedCoverage);
