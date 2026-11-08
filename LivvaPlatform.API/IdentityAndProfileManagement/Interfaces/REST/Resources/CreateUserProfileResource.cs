using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     Data required to create a user profile.
/// </summary>
/// <param name="UserId">The identifier of the owner user.</param>
/// <param name="DocumentType">The identity document type (for example, DNI).</param>
/// <param name="DocumentNumber">The identity document number.</param>
/// <param name="BirthDate">The birth date (yyyy-MM-dd).</param>
/// <param name="Address">The address.</param>
/// <param name="District">The district.</param>
/// <param name="Province">The province.</param>
/// <param name="Department">The department.</param>
/// <param name="Country">The country.</param>
public record CreateUserProfileResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Required(ErrorMessage = "RequiredDocumentType")] [StringLength(30)] string DocumentType,
    [Required(ErrorMessage = "RequiredDocumentNumber")] [StringLength(30)] string DocumentNumber,
    DateOnly? BirthDate,
    [StringLength(255)] string? Address,
    [StringLength(100)] string? District,
    [StringLength(100)] string? Province,
    [StringLength(100)] string? Department,
    [StringLength(100)] string? Country);