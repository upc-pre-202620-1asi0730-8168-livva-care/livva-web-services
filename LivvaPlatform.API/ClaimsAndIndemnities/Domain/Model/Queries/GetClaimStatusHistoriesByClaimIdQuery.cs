namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to get the status history of a vehicle claim.
/// </summary>
/// <param name="ClaimId">The identifier of the vehicle claim.</param>
public record GetClaimStatusHistoriesByClaimIdQuery(long ClaimId);
