using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Interfaces.REST.Resources;

namespace LivvaPlatform.API.NotificationManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles notification resources from the Notification aggregate.
/// </summary>
public static class NotificationResourceFromEntityAssembler
{
    /// <summary>Converts a notification into a resource.</summary>
    /// <param name="e">The notification.</param>
    /// <returns>The notification resource.</returns>
    public static NotificationResource ToResourceFromEntity(Notification e) =>
        new(e.Id, e.UserId, e.Type, e.PolicyId, e.PolicyNumber, e.ExpirationDate, e.IsRead, e.ReadAt, e.CreatedAt, e.UpdatedAt);
}