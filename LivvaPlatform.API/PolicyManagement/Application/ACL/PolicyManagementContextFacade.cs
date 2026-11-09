using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;

namespace LivvaPlatform.API.PolicyManagement.Application.ACL;

/// <summary>
///     Implementation of the Policy Management anti-corruption layer facade.
/// </summary>
/// <param name="policyRepository">Repository for accessing policies.</param>
public class PolicyManagementContextFacade(IPolicyRepository policyRepository) : IPolicyManagementContextFacade
{
    /// <inheritdoc />
    public async Task<bool> PolicyExistsAsync(long policyId, CancellationToken cancellationToken = default) =>
        await policyRepository.FindByIdAsync(policyId, cancellationToken) is not null;
}