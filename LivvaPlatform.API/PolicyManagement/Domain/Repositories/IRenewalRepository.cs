using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the Renewal aggregate.
/// </summary>
public interface IRenewalRepository : IBaseRepository<Renewal>
{
    /// <summary>Finds the renewals of a policy.</summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The renewals.</returns>
    Task<IEnumerable<Renewal>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default);
}
