using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.NotificationManagement.Interfaces.REST.Resources;

/// <summary>Data required to create a notification. New notifications are always unread.</summary>
public record CreateNotificationResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Required(ErrorMessage = "RequiredNotificationType")] [StringLength(50)] string Type,
    long? PolicyId,
    [StringLength(50)] string? PolicyNumber,
    DateOnly? ExpirationDate);
