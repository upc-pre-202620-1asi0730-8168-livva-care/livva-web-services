namespace LivvaPlatform.API.NotificationManagement.Application.Errors;

/// <summary>
///     Errors that can occur when managing notifications.
/// </summary>
public enum NotificationError
{
    /// <summary>The recipient user does not exist.</summary>
    UserNotFound,

    /// <summary>The related policy does not exist.</summary>
    PolicyNotFound,

    /// <summary>The user already has an expiration notice for the policy.</summary>
    NotificationAlreadyExists,

    /// <summary>The notification does not exist.</summary>
    NotificationNotFound,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}