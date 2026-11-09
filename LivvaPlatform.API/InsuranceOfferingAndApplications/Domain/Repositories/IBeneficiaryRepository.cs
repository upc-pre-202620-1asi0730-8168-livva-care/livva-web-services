using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the Beneficiary aggregate.
/// </summary>
public interface IBeneficiaryRepository : IBaseRepository<Beneficiary>
{
    /// <summary>Finds the beneficiaries of a policy.</summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The beneficiaries.</returns>
    Task<IEnumerable<Beneficiary>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default);
}
