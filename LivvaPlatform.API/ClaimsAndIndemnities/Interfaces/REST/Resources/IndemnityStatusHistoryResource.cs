namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a status change in an indemnity request.
/// </summary>
public record IndemnityStatusHistoryResource(
    long Id,
    string Status,
    string Comment,
    DateTimeOffset ChangedAt);