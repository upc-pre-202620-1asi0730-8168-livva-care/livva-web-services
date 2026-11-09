namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>A policy coverage returned by the API.</summary>
public record PolicyCoverageResource(long Id, long PolicyId, string Name, string? Description, decimal CoverageLimit, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
