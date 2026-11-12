using System.Net.Mime;
using LivvaPlatform.API.NotificationManagement.Application.Errors;
using LivvaPlatform.API.NotificationManagement.Application.Services;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Commands;
using LivvaPlatform.API.NotificationManagement.Domain.Model.Queries;
using LivvaPlatform.API.NotificationManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.NotificationManagement.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.NotificationManagement.Interfaces.REST;

/// <summary>
///     Creates, queries and marks notifications as read. Requires a valid access token.
/// </summary>
/// <param name="commandService">The notification command service.</param>
/// <param name="queryService">The notification query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Notifications")]
public class NotificationsController(
    INotificationCommandService commandService,
    INotificationQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Creates a notification, such as a policy expiration notice. New notifications are unread.</summary>
    /// <param name="resource">The notification data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created notification.</returns>
    /// <response code="201">The notification was created.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The user or the policy does not exist.</response>
    /// <response code="409">The user already has a notification of this type for the policy.</response>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateNotification([FromBody] CreateNotificationResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateNotificationCommand(resource.UserId, resource.Type, resource.PolicyId, resource.PolicyNumber,
            resource.ExpirationDate);
        var result = await commandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Notification, NotificationError>.Success s => CreatedAtAction(nameof(GetNotificationById),
                new { notificationId = s.Value.Id }, NotificationResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
            Result<Notification, NotificationError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets notifications, optionally filtered by user.</summary>
    /// <param name="userId">Optional identifier of the recipient user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching notifications, newest first (empty if none match).</returns>
    /// <response code="200">The matching notifications.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetNotifications([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        var notifications = await queryService.Handle(new GetNotificationsQuery(userId), cancellationToken);
        return Ok(notifications.OrderByDescending(n => n.CreatedAt)
            .Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a notification by its identifier.</summary>
    /// <param name="notificationId">The identifier of the notification.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notification.</returns>
    /// <response code="200">The notification was found.</response>
    /// <response code="404">The notification does not exist.</response>
    [HttpGet("{notificationId:long}")]
    [ProducesResponseType(typeof(NotificationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetNotificationById(long notificationId, CancellationToken cancellationToken)
    {
        var notification = await queryService.Handle(new GetNotificationByIdQuery(notificationId), cancellationToken);
        return notification is null
            ? ToProblem(NotificationError.NotificationNotFound)
            : Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification));
    }

    /// <summary>Marks a notification as read (<c>isRead = true</c>).</summary>
    /// <param name="notificationId">The identifier of the notification.</param>
    /// <param name="resource">The update. Only <c>isRead = true</c> is supported.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated notification.</returns>
    /// <response code="200">The notification was marked as read.</response>
    /// <response code="400">The update is not supported.</response>
    /// <response code="404">The notification does not exist.</response>
    [HttpPatch("{notificationId:long}")]
    [ProducesResponseType(typeof(NotificationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateNotification(long notificationId, [FromBody] UpdateNotificationResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.IsRead != true)
            return Problem(title: localizer["InvalidNotificationUpdate"].Value, statusCode: StatusCodes.Status400BadRequest);

        var result = await commandService.Handle(new MarkNotificationAsReadCommand(notificationId), cancellationToken);
        return result switch
        {
            Result<Notification, NotificationError>.Success s => Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
            Result<Notification, NotificationError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private ObjectResult ToProblem(NotificationError error) => error switch
    {
        NotificationError.UserNotFound => Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        NotificationError.PolicyNotFound => Problem(title: localizer["PolicyNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        NotificationError.NotificationAlreadyExists => Problem(title: localizer["NotificationAlreadyExists"].Value, statusCode: StatusCodes.Status409Conflict),
        NotificationError.NotificationNotFound => Problem(title: localizer["NotificationNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        _ => Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
    };
}