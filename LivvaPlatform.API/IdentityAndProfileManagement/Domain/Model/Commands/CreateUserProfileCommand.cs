namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to create the profile of a user.
/// </summary>
/// <param name="UserId">The identifier of the owner user.</param>
/// <param name="DocumentType">The identity document type (for example, DNI).</param>
/// <param name="DocumentNumber">The identity document number.</param>
/// <param name="BirthDate">The birth date.</param>
/// <param name="Address">The address.</param>
/// <param name="District">The district.</param>
/// <param name="Province">The province.</param>
/// <param name="Department">The department.</param>
/// <param name="Country">The country.</param>
public record CreateUserProfileCommand(
    long UserId,
    string DocumentType,
    string DocumentNumber,
    DateOnly? BirthDate,
    string? Address,
    string? District,
    string? Province,
    string? Department,
    string? Country);