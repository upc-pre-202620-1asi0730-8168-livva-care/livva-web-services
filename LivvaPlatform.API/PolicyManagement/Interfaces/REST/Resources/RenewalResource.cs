namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>A renewal returned by the API. The status is <c>pending</c>, <c>under_review</c>, <c>approved</c> or <c>rejected</c>.</summary>
public record RenewalResource(long Id, long PolicyId, DateTimeOffset RequestedAt, DateOnly? ProposedExpirationDate, decimal? ProposedPremium, string Status, DateTimeOffset? ResolvedAt, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
