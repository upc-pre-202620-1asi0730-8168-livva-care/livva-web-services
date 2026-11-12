using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LivvaPlatform.API.SubscriptionManagement.Application.Internal.OutboundServices;
using Microsoft.Extensions.Options;

namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Payments.MercadoPago.Services;

/// <summary>
///     Creates orders through the Mercado Pago Orders API (<c>POST /v1/orders</c>).
/// </summary>
/// <param name="httpClient">The HTTP client configured with the Mercado Pago base URL.</param>
/// <param name="options">The Mercado Pago settings.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class MercadoPagoPaymentService(
    HttpClient httpClient,
    IOptions<MercadoPagoSettings> options,
    ILogger<MercadoPagoPaymentService> logger) : IPaymentProviderService
{
    /// <inheritdoc />
    public bool IsConfigured => !string.IsNullOrWhiteSpace(options.Value.AccessToken);

    /// <inheritdoc />
    public async Task<PaymentOrderResult> CreateOrderAsync(JsonElement order, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AccessToken);
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());
        request.Content = new StringContent(order.GetRawText(), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        JsonElement body;
        try
        {
            body = JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content).RootElement.Clone();
        }
        catch (JsonException)
        {
            body = JsonDocument.Parse("{}").RootElement.Clone();
        }

        if (!response.IsSuccessStatusCode)
            logger.LogWarning("Mercado Pago rejected the order with status {StatusCode}", (int)response.StatusCode);
        return new PaymentOrderResult((int)response.StatusCode, body);
    }
}