namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;

/// <summary>
///     Errors that can occur when creating or updating a user profile.
/// </summary>
public enum UserProfileError
{
    /// <summary>
    ///     The owner user does not exist.
    /// </summary>
    UserNotFound,

    /// <summary>
    ///     The user already has a profile.
    /// </summary>
    ProfileAlreadyExists,

    /// <summary>
    ///     The profile does not exist.
    /// </summary>
    ProfileNotFound,

    /// <summary>
    ///     The provided data is not valid.
    /// </summary>
    InvalidData
}