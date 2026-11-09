using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the Policy aggregate.
/// </summary>
public interface IPolicyRepository : IBaseRepository<Policy>
{
    /// <summary>Finds the policies of a user.</summary>
    /// <param name="userId">The identifier of the policyholder.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The policies.</returns>
    Task<IEnumerable<Policy>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether a policy number is already used.</summary>
    /// <param name="policyNumber">The normalized policy number.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the number is used.</returns>
    Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
}
