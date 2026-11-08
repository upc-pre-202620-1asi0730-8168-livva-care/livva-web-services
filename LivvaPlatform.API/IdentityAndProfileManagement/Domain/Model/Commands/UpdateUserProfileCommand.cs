namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to partially update the profile of a user.
/// </summary>
/// <remarks>Null values keep the current value.</remarks>
/// <param name="UserProfileId">The identifier of the profile.</param>
/// <param name="DocumentType">The identity document type.</param>
/// <param name="DocumentNumber">The identity document number.</param>
/// <param name="BirthDate">The birth date.</param>
/// <param name="Address">The address.</param>
/// <param name="District">The district.</param>
/// <param name="Province">The province.</param>
/// <param name="Department">The department.</param>
/// <param name="Country">The country.</param>
public record UpdateUserProfileCommand(
    long UserProfileId,
    string? DocumentType,
    string? DocumentNumber,
    DateOnly? BirthDate,
    string? Address,
    string? District,
    string? Province,
    string? Department,
    string? Country);