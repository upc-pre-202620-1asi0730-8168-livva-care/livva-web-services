using System.Text.Json;

namespace LivvaPlatform.API.SubscriptionManagement.Application.Internal.OutboundServices;

/// <summary>
///     Result returned by the payment provider when an order is created.
/// </summary>
/// <param name="StatusCode">The HTTP status code returned by the provider.</param>
/// <param name="Body">The JSON body returned by the provider.</param>
public record PaymentOrderResult(int StatusCode, JsonElement Body);

/// <summary>
///     Creates payment orders in the external payment provider.
/// </summary>
public interface IPaymentProviderService
{
    /// <summary>
    ///     Gets a value indicating whether the provider credentials are configured.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    ///     Creates a payment order with the request body expected by the provider.
    /// </summary>
    /// <param name="order">The order request body.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The provider response.</returns>
    Task<PaymentOrderResult> CreateOrderAsync(JsonElement order, CancellationToken cancellationToken = default);
}