namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;

/// <summary>
///     Query to get policy coverages, optionally filtered by policy.
/// </summary>
public record GetPolicyCoveragesQuery(long? PolicyId);
