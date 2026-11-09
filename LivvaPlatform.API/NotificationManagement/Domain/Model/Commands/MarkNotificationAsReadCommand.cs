namespace LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;

/// <summary>
///     Command to mark a notification as read.
/// </summary>
public record MarkNotificationAsReadCommand(long NotificationId);
