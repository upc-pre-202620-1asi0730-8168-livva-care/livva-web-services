namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;

/// <summary>
///     Errors that can occur when authenticating a user.
/// </summary>
public enum SignInError
{
    /// <summary>
    ///     The email does not exist, the password is wrong or the account is inactive.
    /// </summary>
    InvalidCredentials
}