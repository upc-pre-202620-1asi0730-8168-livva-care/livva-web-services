using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.NotificationManagement.Application.Errors;
using LivvaPlatform.API.NotificationManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;
using LivvaPlatform.API.NotificationManagement.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LivvaPlatform.API.Tests.NotificationManagement;

/// <summary>
///     Unit tests for the Notification Management bounded context.
/// </summary>
public class NotificationManagementTests
{
    private readonly Mock<INotificationRepository> _notifications = new();
    private readonly Mock<IIamContextFacade> _iam = new();
    private readonly Mock<IPolicyManagementContextFacade> _policies = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private NotificationCommandService Service() => new(_notifications.Object, _iam.Object, _policies.Object,
        _unitOfWork.Object, NullLogger<NotificationCommandService>.Instance);

    private static CreateNotificationCommand Command() =>
        new(1, "Policy_Expiration", 1, "LIV-VEH-0001", new DateOnly(2027, 1, 14));

    [Fact]
    public void Notification_ShouldStartUnreadWithNormalizedType()
    {
        var notification = new Notification(Command());

        Assert.False(notification.IsRead);
        Assert.Null(notification.ReadAt);
        Assert.Equal(Notification.PolicyExpirationType, notification.Type);
    }

    [Fact]
    public void MarkAsRead_ShouldKeepFirstReadTimestamp()
    {
        var notification = new Notification(Command());

        notification.MarkAsRead();
        var firstReadAt = notification.ReadAt;
        notification.MarkAsRead();

        Assert.True(notification.IsRead);
        Assert.Equal(firstReadAt, notification.ReadAt);
    }

    [Fact]
    public async Task Create_ForUnknownPolicy_ShouldFail()
    {
        _iam.Setup(i => i.UserExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await Service().Handle(Command());

        Assert.Equal(NotificationError.PolicyNotFound,
            Assert.IsType<Result<Notification, NotificationError>.Failure>(result).Error);
    }

    [Fact]
    public async Task Create_WhenNoticeAlreadyExists_ShouldFail()
    {
        _iam.Setup(i => i.UserExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _policies.Setup(p => p.PolicyExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _notifications.Setup(n => n.ExistsAsync(1, "policy_expiration", 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await Service().Handle(Command());

        Assert.Equal(NotificationError.NotificationAlreadyExists,
            Assert.IsType<Result<Notification, NotificationError>.Failure>(result).Error);
    }
}