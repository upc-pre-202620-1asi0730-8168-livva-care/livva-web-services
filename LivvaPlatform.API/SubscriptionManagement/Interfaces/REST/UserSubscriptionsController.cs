using System.Net.Mime;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Application.Services;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;
using LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST;
/// <summary>
///     Creates, queries, cancels and deletes user subscriptions. Requires a valid access token.
/// </summary>
/// <param name="commandService">The subscription command service.</param>
/// <param name="queryService">The subscription query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("User Subscriptions")]
public class UserSubscriptionsController(
    ISubscriptionCommandService commandService,
    ISubscriptionQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Subscribes a user to a plan after its payment was approved.</summary>
    /// <param name="resource">The subscription data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created subscription.</returns>
    /// <response code="201">The subscription was created.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The user does not exist or the plan is not available.</response>
    /// <response code="409">The user already has an active subscription.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserSubscriptionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateUserSubscription([FromBody] CreateUserSubscriptionResource resource,
        CancellationToken cancellationToken)
    {
        if (!EnumValueConverter.TryParse<BillingCycle>(resource.BillingCycle, out var billingCycle))
            return Problem(title: localizer["InvalidBillingCycle"].Value, statusCode: StatusCodes.Status400BadRequest);

        var result = await commandService.Handle(new CreateUserSubscriptionCommand(resource.UserId, resource.PlanId, billingCycle),
            cancellationToken);
        return result switch
        {
            Result<UserSubscription, SubscriptionError>.Success s => CreatedAtAction(nameof(GetUserSubscriptionById),
                new { userSubscriptionId = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, SubscriptionError>.Failure f => SubscriptionProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets user subscriptions, optionally filtered by user.</summary>
    /// <param name="userId">Optional identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching subscriptions (empty if none match).</returns>
    /// <response code="200">The matching subscriptions.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserSubscriptionResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUserSubscriptions([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        var subscriptions = await queryService.Handle(new GetUserSubscriptionsQuery(userId), cancellationToken);
        return Ok(subscriptions.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a user subscription by its identifier.</summary>
    /// <param name="userSubscriptionId">The identifier of the subscription.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The subscription.</returns>
    /// <response code="200">The subscription was found.</response>
    /// <response code="404">The subscription does not exist.</response>
    [HttpGet("{userSubscriptionId:long}")]
    [ProducesResponseType(typeof(UserSubscriptionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserSubscriptionById(long userSubscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await queryService.Handle(new GetUserSubscriptionByIdQuery(userSubscriptionId), cancellationToken);
        return subscription is null
            ? SubscriptionProblemMapper.ToProblem(this, localizer, SubscriptionError.SubscriptionNotFound)
            : Ok(ResourceAssembler.ToResource(subscription));
    }

    /// <summary>Cancels an active subscription by setting its status to <c>cancelled</c>.</summary>
    /// <param name="userSubscriptionId">The identifier of the subscription.</param>
    /// <param name="resource">The new status. Only <c>cancelled</c> is supported.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The cancelled subscription.</returns>
    /// <response code="200">The subscription was cancelled.</response>
    /// <response code="400">The status is not supported.</response>
    /// <response code="404">The subscription does not exist.</response>
    /// <response code="409">The subscription is not active.</response>
    [HttpPatch("{userSubscriptionId:long}")]
    [ProducesResponseType(typeof(UserSubscriptionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateUserSubscription(long userSubscriptionId, [FromBody] UpdateUserSubscriptionResource resource,
        CancellationToken cancellationToken)
    {
        if (!EnumValueConverter.TryParse<SubscriptionStatus>(resource.Status, out var status) || status != SubscriptionStatus.CANCELLED)
            return Problem(title: localizer["InvalidSubscriptionStatus"].Value, statusCode: StatusCodes.Status400BadRequest);

        var result = await commandService.Handle(new CancelUserSubscriptionCommand(userSubscriptionId), cancellationToken);
        return result switch
        {
            Result<UserSubscription, SubscriptionError>.Success s => Ok(ResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, SubscriptionError>.Failure f => SubscriptionProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Deletes a subscription and its payments.</summary>
    /// <param name="userSubscriptionId">The identifier of the subscription.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The subscription was deleted.</response>
    /// <response code="404">The subscription does not exist.</response>
    [HttpDelete("{userSubscriptionId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserSubscription(long userSubscriptionId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new DeleteUserSubscriptionCommand(userSubscriptionId), cancellationToken);
        return result switch
        {
            Result<UserSubscription, SubscriptionError>.Success => NoContent(),
            Result<UserSubscription, SubscriptionError>.Failure f => SubscriptionProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}