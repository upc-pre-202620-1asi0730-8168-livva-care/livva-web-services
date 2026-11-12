namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     A user profile returned by the API.
/// </summary>
/// <param name="Id">The identifier of the profile.</param>
/// <param name="UserId">The identifier of the owner user.</param>
/// <param name="DocumentType">The identity document type.</param>
/// <param name="DocumentNumber">The identity document number.</param>
/// <param name="BirthDate">The birth date.</param>
/// <param name="Address">The address.</param>
/// <param name="District">The district.</param>
/// <param name="Province">The province.</param>
/// <param name="Department">The department.</param>
/// <param name="Country">The country.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record UserProfileResource(
    long Id,
    long UserId,
    string DocumentType,
    string DocumentNumber,
    DateOnly? BirthDate,
    string? Address,
    string? District,
    string? Province,
    string? Department,
    string? Country,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);