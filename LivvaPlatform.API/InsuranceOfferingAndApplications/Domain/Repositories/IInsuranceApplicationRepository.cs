using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the InsuranceApplication aggregate.
/// </summary>
public interface IInsuranceApplicationRepository : IBaseRepository<InsuranceApplication>
{
    /// <summary>Finds the applications of a user.</summary>
    /// <param name="userId">The identifier of the applicant.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The applications.</returns>
    Task<IEnumerable<InsuranceApplication>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
