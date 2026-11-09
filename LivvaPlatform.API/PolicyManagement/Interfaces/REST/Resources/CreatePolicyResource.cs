using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>Data of a policy issued by an insurance company. The insurance type is <c>vehicle</c> or <c>life</c>.</summary>
public record CreatePolicyResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "RequiredApplicationId")] long ApplicationId,
    [Required(ErrorMessage = "RequiredPolicyNumber")] [StringLength(50)] string PolicyNumber,
    [Required(ErrorMessage = "InvalidInsuranceType")] string InsuranceType,
    DateOnly StartDate,
    DateOnly ExpirationDate,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal PremiumAmount,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal CoverageAmount);
