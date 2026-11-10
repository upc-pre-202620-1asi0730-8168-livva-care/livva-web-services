using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a life insurance indemnity request.
/// </summary>
public record IndemnityRequestResource(
    long Id,
    long PolicyId,
    long BeneficiaryId,
    DateOnly RequestDate,
    string Description,
    IndemnityRequestStatus Status,
    decimal? IndemnityAmount,
    DateTimeOffset? ResolvedAt,
    IEnumerable<IndemnityStatusHistoryResource> StatusHistories);