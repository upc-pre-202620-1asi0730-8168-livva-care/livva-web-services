namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     The authenticated user and its access token.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="FirstName">The first name.</param>
/// <param name="LastName">The last name.</param>
/// <param name="Email">The email address.</param>
/// <param name="Phone">The phone number.</param>
/// <param name="Role">The role.</param>
/// <param name="Active">Whether the account is active.</param>
/// <param name="Token">The JWT to send in the Authorization header as "Bearer {token}".</param>
public record AuthenticatedUserResource(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    bool Active,
    string Token);