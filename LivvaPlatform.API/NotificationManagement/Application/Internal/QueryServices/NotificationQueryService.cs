using LivvaPlatform.API.NotificationManagement.Application.Services;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Queries;
using LivvaPlatform.API.NotificationManagement.Domain.Repositories;

namespace LivvaPlatform.API.NotificationManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying notifications.
/// </summary>
/// <param name="notificationRepository">Repository for accessing notifications.</param>
public class NotificationQueryService(INotificationRepository notificationRepository) : INotificationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Notification>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken = default) =>
        query.UserId is null
            ? await notificationRepository.ListAsync(cancellationToken)
            : await notificationRepository.FindByUserIdAsync(query.UserId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<Notification?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByIdAsync(query.NotificationId, cancellationToken);
}