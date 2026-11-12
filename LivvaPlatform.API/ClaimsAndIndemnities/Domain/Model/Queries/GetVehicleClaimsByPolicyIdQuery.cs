namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to get the vehicle claims associated with a policy.
/// </summary>
public record GetVehicleClaimsByPolicyIdQuery(long PolicyId);
