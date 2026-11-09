namespace LivvaPlatform.API.NotificationManagement.Interfaces.REST.Resources;

/// <summary>Partial update of a notification. Only <c>isRead = true</c> is supported.</summary>
public record UpdateNotificationResource(bool? IsRead);
