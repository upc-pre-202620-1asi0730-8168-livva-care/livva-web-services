namespace LivvaPlatform.API.NotificationManagement.Interfaces.REST.Resources;

/// <summary>A notification returned by the API.</summary>
public record NotificationResource(long Id, long UserId, string Type, long? PolicyId, string? PolicyNumber, DateOnly? ExpirationDate, bool IsRead, DateTimeOffset? ReadAt, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
