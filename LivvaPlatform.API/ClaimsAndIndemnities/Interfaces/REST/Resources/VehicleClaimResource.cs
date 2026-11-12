namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     A vehicle claim returned by the API. The status is <c>reported</c>, <c>under_review</c>,
///     <c>approved</c>, <c>rejected</c>, <c>resolved</c> or <c>cancelled</c>.
/// </summary>
public record VehicleClaimResource(
    long Id,
    long PolicyId,
    long UserId,
    DateOnly IncidentDate,
    string IncidentLocation,
    string Description,
    decimal EstimatedDamageAmount,
    string Status,
    decimal? CompensationAmount,
    DateTimeOffset ReportedAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
