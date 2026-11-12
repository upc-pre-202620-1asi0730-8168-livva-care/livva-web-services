namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;

/// <summary>
///     Command to register a vehicle insurance claim.
/// </summary>
/// <param name="PolicyId">The identifier of the vehicle insurance policy.</param>
/// <param name="UserId">The identifier of the insured user who reports the claim.</param>
/// <param name="IncidentDate">The date of the incident.</param>
/// <param name="IncidentLocation">The place where the incident happened.</param>
/// <param name="Description">The description of the incident.</param>
/// <param name="EstimatedDamageAmount">The estimated amount of the damage.</param>
public record CreateVehicleClaimCommand(
    long PolicyId,
    long UserId,
    DateOnly IncidentDate,
    string IncidentLocation,
    string Description,
    decimal EstimatedDamageAmount);
