using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles queries for vehicle claims.
/// </summary>
public interface IVehicleClaimQueryService
{
    /// <summary>Gets a vehicle claim by its identifier.</summary>
    Task<VehicleClaim?> Handle(
        GetVehicleClaimByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle claims associated with a user.</summary>
    Task<IEnumerable<VehicleClaim>> Handle(
        GetVehicleClaimsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle claims associated with a policy.</summary>
    Task<IEnumerable<VehicleClaim>> Handle(
        GetVehicleClaimsByPolicyIdQuery query,
        CancellationToken cancellationToken = default);
}