using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

/// <summary>
///     Repository of vehicle claims.
/// </summary>
public interface IVehicleClaimRepository : IBaseRepository<VehicleClaim>
{
    /// <summary>Finds the vehicle claims reported by a user, newest first.</summary>
    Task<IEnumerable<VehicleClaim>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Finds the vehicle claims of a policy, newest first.</summary>
    Task<IEnumerable<VehicleClaim>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default);

    /// <summary>Finds the status history of a vehicle claim, newest first.</summary>
    Task<IEnumerable<ClaimStatusHistory>> FindStatusHistoriesByClaimIdAsync(long claimId,
        CancellationToken cancellationToken = default);
}
