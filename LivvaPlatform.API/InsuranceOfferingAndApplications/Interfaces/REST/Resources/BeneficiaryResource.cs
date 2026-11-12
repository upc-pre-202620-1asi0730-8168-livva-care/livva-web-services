namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>A beneficiary returned by the API.</summary>
public record BeneficiaryResource(long Id, long PolicyId, string FullName, string DocumentType, string DocumentNumber, string Relationship, DateOnly? BirthDate, decimal ParticipationPercentage, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
