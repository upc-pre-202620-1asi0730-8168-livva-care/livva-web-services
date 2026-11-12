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
    string Status,
    decimal? IndemnityAmount,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? ResolvedAt,
    IEnumerable<IndemnityStatusHistoryResource> StatusHistories);