namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;

/// <summary>
///     Query to get policy documents, optionally filtered by policy.
/// </summary>
public record GetPolicyDocumentsQuery(long? PolicyId);
