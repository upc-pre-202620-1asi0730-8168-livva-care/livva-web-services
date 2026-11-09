namespace LivvaPlatform.API.PolicyManagement.Interfaces.ACL;

/// <summary>
///     Anti-corruption layer facade that exposes Policy Management data to other bounded contexts.
/// </summary>
public interface IPolicyManagementContextFacade
{
    /// <summary>
    ///     Determines whether a policy exists.
    /// </summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the policy exists; otherwise, false.</returns>
    Task<bool> PolicyExistsAsync(long policyId, CancellationToken cancellationToken = default);
}