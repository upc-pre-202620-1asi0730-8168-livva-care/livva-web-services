namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>An insurance application returned by the API. The status is <c>pending</c>, <c>under_review</c>, <c>approved</c>, <c>rejected</c> or <c>cancelled</c>.</summary>
public record InsuranceApplicationResource(long Id, long UserId, long ProductId, decimal RequestedCoverage, string Status, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
