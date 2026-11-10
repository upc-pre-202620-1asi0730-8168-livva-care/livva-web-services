namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to get indemnity requests associated with a policy.
/// </summary>
public record GetIndemnityRequestsByPolicyIdQuery(long PolicyId);