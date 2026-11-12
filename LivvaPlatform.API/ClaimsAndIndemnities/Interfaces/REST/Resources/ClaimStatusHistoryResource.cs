namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     A status history entry of a vehicle claim.
/// </summary>
public record ClaimStatusHistoryResource(
    long Id,
    long ClaimId,
    string Status,
    string Comment,
    DateTimeOffset ChangedAt,
    DateTimeOffset CreatedAt);
