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
///     Queries the Livva subscription plans. Requires a valid access token.
/// </summary>
/// <param name="queryService">The subscription query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscription Plans")]
public class SubscriptionPlansController(ISubscriptionQueryService queryService, IStringLocalizer<SharedResource> localizer)
    : ControllerBase
{
    /// <summary>Gets subscription plans, optionally only the active ones.</summary>
    /// <param name="active">Optional filter by active status.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching plans.</returns>
    /// <response code="200">The matching plans.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSubscriptionPlans([FromQuery] bool? active, CancellationToken cancellationToken)
    {
        var plans = await queryService.Handle(new GetSubscriptionPlansQuery(active), cancellationToken);
        return Ok(plans.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a subscription plan by its identifier.</summary>
    /// <param name="planId">The identifier of the plan.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The plan.</returns>
    /// <response code="200">The plan was found.</response>
    /// <response code="404">The plan does not exist.</response>
    [HttpGet("{planId:long}")]
    [ProducesResponseType(typeof(SubscriptionPlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSubscriptionPlanById(long planId, CancellationToken cancellationToken)
    {
        var plan = await queryService.Handle(new GetSubscriptionPlanByIdQuery(planId), cancellationToken);
        return plan is null
            ? Problem(title: localizer["SubscriptionPlanNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(plan));
    }
}

/// <summary>
///     Maps subscription errors to localized ProblemDetails responses.
/// </summary>
public static class SubscriptionProblemMapper
{
    /// <summary>Creates the ProblemDetails response for an error.</summary>
    /// <param name="controller">The controller that produces the response.</param>
    /// <param name="localizer">The string localizer.</param>
    /// <param name="error">The subscription error.</param>
    /// <returns>The ProblemDetails response.</returns>
    public static ObjectResult ToProblem(ControllerBase controller, IStringLocalizer<SharedResource> localizer, SubscriptionError error) =>
        error switch
        {
            SubscriptionError.UserNotFound => controller.Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
            SubscriptionError.PlanNotAvailable => controller.Problem(title: localizer["SubscriptionPlanNotAvailable"].Value, statusCode: StatusCodes.Status404NotFound),
            SubscriptionError.ActiveSubscriptionExists => controller.Problem(title: localizer["ActiveSubscriptionExists"].Value, statusCode: StatusCodes.Status409Conflict),
            SubscriptionError.SubscriptionNotFound => controller.Problem(title: localizer["UserSubscriptionNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
            SubscriptionError.SubscriptionNotActive => controller.Problem(title: localizer["SubscriptionNotActive"].Value, statusCode: StatusCodes.Status409Conflict),
            _ => controller.Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
        };
}