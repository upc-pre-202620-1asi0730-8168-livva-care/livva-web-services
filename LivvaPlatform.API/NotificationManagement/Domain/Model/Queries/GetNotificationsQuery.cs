namespace LivvaPlatform.API.NotificationManagement.Domain.Model.Queries;

/// <summary>
///     Query to get notifications, optionally filtered by user.
/// </summary>
public record GetNotificationsQuery(long? UserId);
