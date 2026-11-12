namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Payments.MercadoPago;

/// <summary>
///     Settings of the Mercado Pago integration.
/// </summary>
/// <remarks>
///     The access token is read from the <c>MERCADO_PAGO_ACCESS_TOKEN</c> environment variable or from
///     <c>MercadoPago:AccessToken</c> (user secrets in development). It is never stored in the repository.
/// </remarks>
public class MercadoPagoSettings
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "MercadoPago";

    /// <summary>Gets or sets the base URL of the Mercado Pago API.</summary>
    public string BaseUrl { get; set; } = "https://api.mercadopago.com/";

    /// <summary>Gets or sets the access token.</summary>
    public string AccessToken { get; set; } = string.Empty;
}