namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;

/// <summary>
///     Command to register a vehicle insurance claim.
/// </summary>
public record CreateVehicleClaimCommand(
    long PolicyId,
    long UserId,
    string ClaimNumber,
    DateOnly IncidentDate,
    string Description,
    decimal? CompensationAmount);