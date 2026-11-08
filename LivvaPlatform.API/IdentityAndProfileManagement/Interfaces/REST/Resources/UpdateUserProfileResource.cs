using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     Fields of a user profile that can be partially updated. Omitted fields keep their current value.
/// </summary>
/// <param name="DocumentType">The identity document type.</param>
/// <param name="DocumentNumber">The identity document number.</param>
/// <param name="BirthDate">The birth date (yyyy-MM-dd).</param>
/// <param name="Address">The address.</param>
/// <param name="District">The district.</param>
/// <param name="Province">The province.</param>
/// <param name="Department">The department.</param>
/// <param name="Country">The country.</param>
public record UpdateUserProfileResource(
    [StringLength(30)] string? DocumentType,
    [StringLength(30)] string? DocumentNumber,
    DateOnly? BirthDate,
    [StringLength(255)] string? Address,
    [StringLength(100)] string? District,
    [StringLength(100)] string? Province,
    [StringLength(100)] string? Department,
    [StringLength(100)] string? Country);