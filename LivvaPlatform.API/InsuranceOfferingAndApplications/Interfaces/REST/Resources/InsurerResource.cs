namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>An insurer returned by the API.</summary>
public record InsurerResource(long Id, string ExternalReference, string Name, bool Active, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
