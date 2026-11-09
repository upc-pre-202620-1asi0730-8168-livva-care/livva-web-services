namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to partially update a beneficiary. Null values keep the current value.
/// </summary>
public record UpdateBeneficiaryCommand(long BeneficiaryId, string? FullName, string? DocumentType, string? DocumentNumber, string? Relationship, DateOnly? BirthDate, decimal? ParticipationPercentage);
