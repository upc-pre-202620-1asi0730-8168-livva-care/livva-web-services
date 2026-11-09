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
///     Registers and queries subscription payments. Requires a valid access token.
/// </summary>
/// <param name="commandService">The subscription command service.</param>
/// <param name="queryService">The subscription query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscription Payments")]
public class SubscriptionPaymentsController(
    ISubscriptionCommandService commandService,
    ISubscriptionQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Registers a payment of a subscription processed by the payment provider.</summary>
    /// <param name="resource">The payment data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created payment.</returns>
    /// <response code="201">The payment was registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The subscription does not exist.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionPaymentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateSubscriptionPayment([FromBody] CreateSubscriptionPaymentResource resource,
        CancellationToken cancellationToken)
    {
        if (!EnumValueConverter.TryParse<PaymentStatus>(resource.Status, out var status))
            return Problem(title: localizer["InvalidPaymentStatus"].Value, statusCode: StatusCodes.Status400BadRequest);

        var command = new CreateSubscriptionPaymentCommand(resource.UserSubscriptionId, resource.UserId, resource.PlanId,
            resource.Provider, resource.ProviderPaymentId, resource.Amount, resource.Currency, status, resource.PaidAt);
        var result = await commandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<SubscriptionPayment, SubscriptionError>.Success s => CreatedAtAction(nameof(GetSubscriptionPaymentById),
                new { subscriptionPaymentId = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<SubscriptionPayment, SubscriptionError>.Failure f => SubscriptionProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets subscription payments, optionally filtered by user and subscription.</summary>
    /// <param name="userId">Optional identifier of the user.</param>
    /// <param name="userSubscriptionId">Optional identifier of the subscription.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching payments (empty if none match).</returns>
    /// <response code="200">The matching payments.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPaymentResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSubscriptionPayments([FromQuery] long? userId, [FromQuery] long? userSubscriptionId,
        CancellationToken cancellationToken)
    {
        var payments = await queryService.Handle(new GetSubscriptionPaymentsQuery(userId, userSubscriptionId), cancellationToken);
        return Ok(payments.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a subscription payment by its identifier.</summary>
    /// <param name="subscriptionPaymentId">The identifier of the payment.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The payment.</returns>
    /// <response code="200">The payment was found.</response>
    /// <response code="404">The payment does not exist.</response>
    [HttpGet("{subscriptionPaymentId:long}")]
    [ProducesResponseType(typeof(SubscriptionPaymentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSubscriptionPaymentById(long subscriptionPaymentId, CancellationToken cancellationToken)
    {
        var payment = await queryService.Handle(new GetSubscriptionPaymentByIdQuery(subscriptionPaymentId), cancellationToken);
        return payment is null
            ? Problem(title: localizer["SubscriptionPaymentNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(payment));
    }
}