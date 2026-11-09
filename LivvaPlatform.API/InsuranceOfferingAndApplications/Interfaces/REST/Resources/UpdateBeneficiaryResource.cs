using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>Fields of a beneficiary that can be partially updated. Omitted fields keep their current value.</summary>
public record UpdateBeneficiaryResource(
    [StringLength(200)] string? FullName,
    [StringLength(30)] string? DocumentType,
    [StringLength(30)] string? DocumentNumber,
    [StringLength(50)] string? Relationship,
    DateOnly? BirthDate,
    [Range(typeof(decimal), "0.01", "100", ErrorMessage = "InvalidParticipationPercentage")] decimal? ParticipationPercentage);
