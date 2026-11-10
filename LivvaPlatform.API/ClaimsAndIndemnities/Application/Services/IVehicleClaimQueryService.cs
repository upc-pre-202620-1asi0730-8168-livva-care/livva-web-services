using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles queries for vehicle claims.
/// </summary>
public interface IVehicleClaimQueryService
{
    /// <summary>Gets a vehicle claim by its identifier.</summary>
    Task<Result<VehicleClaim, VehicleClaimError>> Handle(
        GetVehicleClaimByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle claims associated with a user.</summary>
    Task<Result<IEnumerable<VehicleClaim>, VehicleClaimError>> Handle(
        GetVehicleClaimsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle claims associated with a policy.</summary>
    Task<Result<IEnumerable<VehicleClaim>, VehicleClaimError>> Handle(
        GetVehicleClaimsByPolicyIdQuery query,
        CancellationToken cancellationToken = default);
}