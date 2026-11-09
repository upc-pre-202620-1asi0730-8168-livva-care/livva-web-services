using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;

/// <summary>
///     Command to change the status of a vehicle insurance claim.
/// </summary>
/// <param name="ClaimId">The identifier of the vehicle claim.</param>
/// <param name="Status">The new status assigned to the vehicle claim.</param>
/// <param name="Comment">The comment explaining the status change.</param>
public record ChangeVehicleClaimStatusCommand(
    long ClaimId,
    VehicleClaimStatus Status,
    string Comment);