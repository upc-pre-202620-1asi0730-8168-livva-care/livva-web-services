using LivvaPlatform.API.NotificationManagement.Application.Errors;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.NotificationManagement.Application.Services;

/// <summary>
///     Handles the commands of the Notification aggregate.
/// </summary>
public interface INotificationCommandService
{
    /// <summary>Creates a notification.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created notification or an error.</returns>
    Task<Result<Notification, NotificationError>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default);

    /// <summary>Marks a notification as read.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated notification or an error.</returns>
    Task<Result<Notification, NotificationError>> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken = default);
}