using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Query service of vehicle claims.
/// </summary>
public interface IVehicleClaimQueryService
{
    /// <summary>Gets a vehicle claim by its identifier.</summary>
    Task<VehicleClaim?> Handle(GetVehicleClaimByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets the vehicle claims reported by a user.</summary>
    Task<IEnumerable<VehicleClaim>> Handle(GetVehicleClaimsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the vehicle claims of a policy.</summary>
    Task<IEnumerable<VehicleClaim>> Handle(GetVehicleClaimsByPolicyIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the status history of a vehicle claim.</summary>
    Task<IEnumerable<ClaimStatusHistory>> Handle(GetClaimStatusHistoriesByClaimIdQuery query,
        CancellationToken cancellationToken = default);
}
