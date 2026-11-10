using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Data required to register a vehicle insurance claim.
/// </summary>
public record CreateVehicleClaimResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredPolicyId")]
    long PolicyId,

    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")]
    long UserId,

    [Required(ErrorMessage = "RequiredClaimNumber")]
    [StringLength(50, ErrorMessage = "InvalidClaimNumberLength")]
    string ClaimNumber,

    [Required]
    DateOnly IncidentDate,

    [Required(ErrorMessage = "RequiredClaimDescription")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "InvalidClaimDescriptionLength")]
    string Description,

    [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "InvalidCompensationAmount")]
    decimal? CompensationAmount);