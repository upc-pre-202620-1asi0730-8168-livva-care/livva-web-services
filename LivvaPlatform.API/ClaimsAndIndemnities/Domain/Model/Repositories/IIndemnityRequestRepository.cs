using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

/// <summary>
///     Repository contract for indemnity requests.
/// </summary>
public interface IIndemnityRequestRepository : IBaseRepository<IndemnityRequest>
{
    /// <summary>
    ///     Gets indemnity requests associated with a beneficiary.
    /// </summary>
    /// <param name="beneficiaryId">The beneficiary's identifier.</param>
    /// <returns>The indemnity requests associated with the beneficiary.</returns>
    Task<IEnumerable<IndemnityRequest>> FindByBeneficiaryIdAsync(long beneficiaryId);

    /// <summary>
    ///     Gets indemnity requests associated with a policy.
    /// </summary>
    /// <param name="policyId">The policy's identifier.</param>
    /// <returns>The indemnity requests associated with the policy.</returns>
    Task<IEnumerable<IndemnityRequest>> FindByPolicyIdAsync(long policyId);
}