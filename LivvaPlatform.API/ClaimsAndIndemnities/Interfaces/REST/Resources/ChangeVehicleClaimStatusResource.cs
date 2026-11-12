using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Data to change the status of a vehicle claim (for example, <c>{"status": "cancelled"}</c>).
/// </summary>
/// <param name="Status">The new status in lowercase.</param>
/// <param name="Comment">An optional comment about the change.</param>
public record ChangeVehicleClaimStatusResource(
    [Required(ErrorMessage = "RequiredStatus")]
    string Status,
    [StringLength(500, ErrorMessage = "InvalidStatusCommentLength")]
    string? Comment);
