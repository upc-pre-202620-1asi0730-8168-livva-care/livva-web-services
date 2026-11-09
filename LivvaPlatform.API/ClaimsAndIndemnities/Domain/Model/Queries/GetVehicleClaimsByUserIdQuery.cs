namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to get the vehicle claims of a user.
/// </summary>
public record GetVehicleClaimsByUserIdQuery(long UserId);