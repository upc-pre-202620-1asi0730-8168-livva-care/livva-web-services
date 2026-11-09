namespace LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;

/// <summary>
///     Command to create a notification for a user.
/// </summary>
public record CreateNotificationCommand(long UserId, string Type, long? PolicyId, string? PolicyNumber, DateOnly? ExpirationDate);
