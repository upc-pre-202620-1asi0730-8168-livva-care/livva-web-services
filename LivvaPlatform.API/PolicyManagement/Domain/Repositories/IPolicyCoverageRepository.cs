using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the PolicyCoverage aggregate.
/// </summary>
public interface IPolicyCoverageRepository : IBaseRepository<PolicyCoverage>
{
    /// <summary>Finds the coverages of a policy.</summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The coverages.</returns>
    Task<IEnumerable<PolicyCoverage>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default);
}
