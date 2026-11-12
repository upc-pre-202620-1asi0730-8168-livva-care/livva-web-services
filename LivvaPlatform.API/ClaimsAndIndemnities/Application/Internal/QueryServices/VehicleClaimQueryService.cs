using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.QueryServices;

/// <summary>
///     Implementation of <see cref="IVehicleClaimQueryService" />.
/// </summary>
public class VehicleClaimQueryService(IVehicleClaimRepository vehicleClaimRepository) : IVehicleClaimQueryService
{
    /// <inheritdoc />
    public async Task<VehicleClaim?> Handle(GetVehicleClaimByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByIdAsync(query.ClaimId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> Handle(GetVehicleClaimsByUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByUserIdAsync(query.UserId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> Handle(GetVehicleClaimsByPolicyIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindByPolicyIdAsync(query.PolicyId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<ClaimStatusHistory>> Handle(GetClaimStatusHistoriesByClaimIdQuery query,
        CancellationToken cancellationToken = default) =>
        await vehicleClaimRepository.FindStatusHistoriesByClaimIdAsync(query.ClaimId, cancellationToken);
}
