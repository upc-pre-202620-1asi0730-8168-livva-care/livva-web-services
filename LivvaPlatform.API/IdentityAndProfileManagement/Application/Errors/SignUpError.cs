namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;

/// <summary>
///     Errors that can occur when registering a user account.
/// </summary>
public enum SignUpError
{
    /// <summary>
    ///     A user with the same email address already exists.
    /// </summary>
    EmailAlreadyRegistered,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}