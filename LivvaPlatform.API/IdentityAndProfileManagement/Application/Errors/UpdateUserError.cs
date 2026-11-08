namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;

/// <summary>
///     Errors that can occur when updating a user.
/// </summary>
public enum UpdateUserError
{
    /// <summary>
    ///     The user does not exist.
    /// </summary>
    UserNotFound,

    /// <summary>
    ///     The provided data is not valid.
    /// </summary>
    InvalidData
}