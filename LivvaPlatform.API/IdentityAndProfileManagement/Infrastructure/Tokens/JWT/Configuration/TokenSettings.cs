namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Tokens.JWT.Configuration;

/// <summary>
///     Settings used to issue and validate JSON Web Tokens.
/// </summary>
/// <remarks>
///     Bound from the <c>TokenSettings</c> configuration section. The secret must be provided through user secrets
///     in development and through the <c>TokenSettings__Secret</c> environment variable in production.
/// </remarks>
public class TokenSettings
{
    /// <summary>
    ///     The configuration section name.
    /// </summary>
    public const string SectionName = "TokenSettings";

    /// <summary>
    ///     Gets or sets the signing secret. It must have at least 32 characters.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = "livva-platform";

    /// <summary>
    ///     Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = "livva-web-application";

    /// <summary>
    ///     Gets or sets the token lifetime in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 1440;
}