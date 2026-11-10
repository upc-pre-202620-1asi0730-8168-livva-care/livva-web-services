using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

/// <summary>
///     Repository contract for vehicle claims.
/// </summary>
public interface IVehicleClaimRepository : IBaseRepository<VehicleClaim>
{
    /// <summary>
    ///     Gets vehicle claims associated with a user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The vehicle claims associated with the user.</returns>
    Task<IEnumerable<VehicleClaim>> FindByUserIdAsync(long userId);

    /// <summary>
    ///     Gets vehicle claims associated with a policy.
    /// </summary>
    /// <param name="policyId">The policy's identifier.</param>
    /// <returns>The vehicle claims associated with the policy.</returns>
    Task<IEnumerable<VehicleClaim>> FindByPolicyIdAsync(long policyId);
}