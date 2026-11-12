namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     A user returned by the API. The password hash is never included.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="FirstName">The first name.</param>
/// <param name="LastName">The last name.</param>
/// <param name="Email">The email address.</param>
/// <param name="Phone">The phone number.</param>
/// <param name="Role">The role (CUSTOMER, INSURANCE_REPRESENTATIVE or ADMIN).</param>
/// <param name="Active">Whether the account is active.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record UserResource(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    bool Active,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);