namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to authenticate a user.
/// </summary>
/// <param name="Email">The email address of the user.</param>
/// <param name="Password">The plain text password of the user.</param>
public record SignInCommand(string Email, string Password);