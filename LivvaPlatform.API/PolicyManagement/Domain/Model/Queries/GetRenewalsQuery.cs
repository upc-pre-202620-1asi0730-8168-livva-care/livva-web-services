namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;

/// <summary>
///     Query to get renewals, optionally filtered by policy.
/// </summary>
public record GetRenewalsQuery(long? PolicyId);
