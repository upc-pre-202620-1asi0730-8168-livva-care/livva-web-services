using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a vehicle claim status history entry.
/// </summary>
public record ClaimStatusHistoryResource(
    long Id,
    VehicleClaimStatus Status,
    string Comment,
    DateTimeOffset ChangedAt);