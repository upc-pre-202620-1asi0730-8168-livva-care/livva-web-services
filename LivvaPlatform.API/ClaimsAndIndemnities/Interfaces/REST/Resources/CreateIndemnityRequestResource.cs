using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Data required to register a life insurance indemnity request.
/// </summary>
public record CreateIndemnityRequestResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredPolicyId")]
    long PolicyId,

    [Range(1, long.MaxValue, ErrorMessage = "RequiredBeneficiaryId")]
    long BeneficiaryId,

    [Required(ErrorMessage = "RequiredRequestDate")]
    DateOnly RequestDate,

    [Required(ErrorMessage = "RequiredIndemnityDescription")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "InvalidIndemnityDescriptionLength")]
    string Description,

    [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "InvalidIndemnityAmount")]
    decimal? IndemnityAmount);