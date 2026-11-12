using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Data to report a vehicle claim. When <c>userId</c> is omitted, the authenticated user is used.
/// </summary>
/// <param name="PolicyId">The identifier of the vehicle insurance policy.</param>
/// <param name="UserId">The identifier of the insured user (optional).</param>
/// <param name="IncidentDate">The date of the incident.</param>
/// <param name="IncidentLocation">The place where the incident happened.</param>
/// <param name="Description">The description of the incident.</param>
/// <param name="EstimatedDamageAmount">The estimated amount of the damage.</param>
public record CreateVehicleClaimResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredPolicyId")]
    long PolicyId,
    long? UserId,
    [Required] DateOnly IncidentDate,
    [Required(ErrorMessage = "RequiredIncidentLocation")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "InvalidIncidentLocationLength")]
    string IncidentLocation,
    [Required(ErrorMessage = "RequiredClaimDescription")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "InvalidClaimDescriptionLength")]
    string Description,
    [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "InvalidEstimatedDamageAmount")]
    decimal EstimatedDamageAmount);
