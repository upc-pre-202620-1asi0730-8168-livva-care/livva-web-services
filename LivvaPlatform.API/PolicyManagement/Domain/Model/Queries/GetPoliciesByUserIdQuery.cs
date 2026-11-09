namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;

/// <summary>
///     Query to get the policies of a user.
/// </summary>
public record GetPoliciesByUserIdQuery(long UserId);
