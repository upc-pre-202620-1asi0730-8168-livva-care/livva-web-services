namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

/// <summary>
///     Command to register a beneficiary of a life insurance policy.
/// </summary>
public record CreateBeneficiaryCommand(long PolicyId, string FullName, string DocumentType, string DocumentNumber, string Relationship, DateOnly? BirthDate, decimal ParticipationPercentage);
