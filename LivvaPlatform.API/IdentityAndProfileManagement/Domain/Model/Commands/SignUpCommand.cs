namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to register a new user account.
/// </summary>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
/// <param name="Email">The email address used to sign in.</param>
/// <param name="Password">The plain text password, hashed before being stored.</param>
/// <param name="Phone">The phone number of the user.</param>
public record SignUpCommand(string FirstName, string LastName, string Email, string Password, string? Phone);