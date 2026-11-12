namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a vehicle insurance claim by its identifier.
/// </summary>
/// <param name="ClaimId">The unique identifier of the vehicle claim.</param>
public record GetVehicleClaimByIdQuery(long ClaimId);
