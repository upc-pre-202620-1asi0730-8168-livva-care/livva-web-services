namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>A policy returned by the API. The insurance type is <c>vehicle</c> or <c>life</c>; the status is <c>pending</c>, <c>active</c>, <c>expired</c>, <c>cancelled</c> or <c>pending_renewal</c>.</summary>
public record PolicyResource(long Id, long UserId, long ApplicationId, string PolicyNumber, string InsuranceType, DateOnly StartDate, DateOnly ExpirationDate, decimal PremiumAmount, decimal CoverageAmount, string Status, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
