using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Data required to register a beneficiary.</summary>
public record CreateBeneficiaryResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredPolicyId")] long PolicyId,
    [Required(ErrorMessage = "RequiredFullName")] [StringLength(200)] string FullName,
    [Required(ErrorMessage = "RequiredDocumentType")] [StringLength(30)] string DocumentType,
    [Required(ErrorMessage = "RequiredDocumentNumber")] [StringLength(30)] string DocumentNumber,
    [Required(ErrorMessage = "RequiredRelationship")] [StringLength(50)] string Relationship,
    DateOnly? BirthDate,
    [Range(typeof(decimal), "0.01", "100", ErrorMessage = "InvalidParticipationPercentage")] decimal ParticipationPercentage);
