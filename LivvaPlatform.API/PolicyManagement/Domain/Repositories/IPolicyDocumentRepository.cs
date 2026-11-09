using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the PolicyDocument aggregate.
/// </summary>
public interface IPolicyDocumentRepository : IBaseRepository<PolicyDocument>
{
    /// <summary>Finds the documents of a policy.</summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The documents.</returns>
    Task<IEnumerable<PolicyDocument>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default);
}
