using LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;

namespace LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a notice addressed to a user, such as the upcoming expiration of a policy.
/// </summary>
public partial class Notification
{
    /// <summary>The type used for policy expiration notices.</summary>
    public const string PolicyExpirationType = "policy_expiration";

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Notification()
    {
        Type = string.Empty;
    }

    /// <summary>
    ///     Creates an unread notification from a creation command.
    /// </summary>
    /// <param name="command">The create notification command.</param>
    /// <exception cref="ArgumentException">Thrown when the type is empty.</exception>
    public Notification(CreateNotificationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Type);
        UserId = command.UserId;
        Type = command.Type.Trim().ToLowerInvariant();
        PolicyId = command.PolicyId;
        PolicyNumber = string.IsNullOrWhiteSpace(command.PolicyNumber) ? null : command.PolicyNumber.Trim();
        ExpirationDate = command.ExpirationDate;
        IsRead = false;
    }

    /// <summary>Gets the identifier of the notification.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the recipient user.</summary>
    public long UserId { get; private set; }

    /// <summary>Gets the type of notification (for example, policy_expiration).</summary>
    public string Type { get; private set; }

    /// <summary>Gets the identifier of the related policy.</summary>
    public long? PolicyId { get; private set; }

    /// <summary>Gets the number of the related policy.</summary>
    public string? PolicyNumber { get; private set; }

    /// <summary>Gets the expiration date of the related policy.</summary>
    public DateOnly? ExpirationDate { get; private set; }

    /// <summary>Gets a value indicating whether the notification was read.</summary>
    public bool IsRead { get; private set; }

    /// <summary>Gets the timestamp when the notification was read.</summary>
    public DateTimeOffset? ReadAt { get; private set; }

    /// <summary>Marks the notification as read. Marking it again keeps the original read timestamp.</summary>
    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTimeOffset.UtcNow;
    }
}