namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>An insurance product returned by the API. The insurance type is <c>vehicle</c> or <c>life</c>.</summary>
public record InsuranceProductResource(long Id, long InsurerId, string Name, string InsuranceType, decimal CoverageAmount, decimal PremiumAmount, bool Active, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
