using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Queries;

namespace LivvaPlatform.API.NotificationManagement.Application.Services;

/// <summary>
///     Handles the queries of the Notification aggregate.
/// </summary>
public interface INotificationQueryService
{
    /// <summary>Gets notifications, optionally filtered by user.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notifications.</returns>
    Task<IEnumerable<Notification>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a notification by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notification if found; otherwise, null.</returns>
    Task<Notification?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default);
}