using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a status change in an indemnity request.
/// </summary>
public record IndemnityStatusHistoryResource(
    long Id,
    IndemnityRequestStatus Status,
    string Comment,
    DateTimeOffset ChangedAt);