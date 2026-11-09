using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.NotificationManagement.Application.Errors;
using LivvaPlatform.API.NotificationManagement.Application.Services;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;
using LivvaPlatform.API.NotificationManagement.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.NotificationManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service that creates notifications and marks them as read.
/// </summary>
/// <param name="notificationRepository">Repository for accessing notifications.</param>
/// <param name="iamContextFacade">Facade used to verify that the recipient exists.</param>
/// <param name="policyManagementContextFacade">Facade used to verify that the related policy exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class NotificationCommandService(
    INotificationRepository notificationRepository,
    IIamContextFacade iamContextFacade,
    IPolicyManagementContextFacade policyManagementContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<NotificationCommandService> logger) : INotificationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Notification, NotificationError>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<Notification, NotificationError>.Failure(NotificationError.UserNotFound);

        if (command.PolicyId is not null)
        {
            if (!await policyManagementContextFacade.PolicyExistsAsync(command.PolicyId.Value, cancellationToken))
                return new Result<Notification, NotificationError>.Failure(NotificationError.PolicyNotFound);
            if (await notificationRepository.ExistsAsync(command.UserId, command.Type.Trim().ToLowerInvariant(),
                    command.PolicyId.Value, cancellationToken))
                return new Result<Notification, NotificationError>.Failure(NotificationError.NotificationAlreadyExists);
        }

        Notification notification;
        try
        {
            notification = new Notification(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while creating a notification for user {UserId}", command.UserId);
            return new Result<Notification, NotificationError>.Failure(NotificationError.InvalidData);
        }

        await notificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Notification, NotificationError>.Success(notification);
    }

    /// <inheritdoc />
    public async Task<Result<Notification, NotificationError>> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId, cancellationToken);
        if (notification is null)
            return new Result<Notification, NotificationError>.Failure(NotificationError.NotificationNotFound);

        notification.MarkAsRead();
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Notification, NotificationError>.Success(notification);
    }
}