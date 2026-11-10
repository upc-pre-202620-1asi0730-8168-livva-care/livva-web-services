using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying vehicle claims.
/// </summary>
/// <param name="vehicleClaimRepository">Repository for accessing vehicle claims.</param>
public class VehicleClaimQueryService(
    IVehicleClaimRepository vehicleClaimRepository) : IVehicleClaimQueryService
{
    /// <inheritdoc />
    public async Task<VehicleClaim?> Handle(
        GetVehicleClaimByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByIdAsync(query.ClaimId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> Handle(
        GetVehicleClaimsByUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByUserIdAsync(query.UserId);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> Handle(
        GetVehicleClaimsByPolicyIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByPolicyIdAsync(query.PolicyId);
}