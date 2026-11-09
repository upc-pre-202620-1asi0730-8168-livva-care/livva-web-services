using System.Net.Mime;
using System.Text.Json;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.SubscriptionManagement.Application.Internal.OutboundServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST;

/// <summary>
///     Creates payment orders in Mercado Pago on behalf of the Web Application, keeping the access token on the server.
/// </summary>
/// <param name="paymentProviderService">The payment provider service.</param>
/// <param name="localizer">String localizer for response messages.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Payment Orders")]
public class PaymentOrdersController(
    IPaymentProviderService paymentProviderService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<PaymentOrdersController> logger) : ControllerBase
{
    /// <summary>
    ///     Creates an order in the Mercado Pago Orders API and returns the provider response unchanged.
    /// </summary>
    /// <param name="order">The order body expected by Mercado Pago (<c>POST /v1/orders</c>).</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The response of Mercado Pago with its status code.</returns>
    /// <response code="201">The order was created by Mercado Pago.</response>
    /// <response code="500">Mercado Pago is not configured.</response>
    /// <response code="502">Mercado Pago could not be reached.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult> CreatePaymentOrder([FromBody] JsonElement order, CancellationToken cancellationToken)
    {
        if (!paymentProviderService.IsConfigured)
            return Problem(title: localizer["PaymentProviderNotConfigured"].Value, statusCode: StatusCodes.Status500InternalServerError);

        try
        {
            var result = await paymentProviderService.CreateOrderAsync(order, cancellationToken);
            return StatusCode(result.StatusCode, result.Body);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Mercado Pago could not be reached");
            return Problem(title: localizer["PaymentProviderUnavailable"].Value, statusCode: StatusCodes.Status502BadGateway);
        }
    }
}