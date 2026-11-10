using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a vehicle insurance claim.
/// </summary>
public record VehicleClaimResource(
    long Id,
    long PolicyId,
    long UserId,
    string ClaimNumber,
    DateOnly IncidentDate,
    string Description,
    VehicleClaimStatus Status,
    decimal? CompensationAmount,
    DateTimeOffset? ResolvedAt,
    IEnumerable<ClaimStatusHistoryResource> StatusHistories);
    