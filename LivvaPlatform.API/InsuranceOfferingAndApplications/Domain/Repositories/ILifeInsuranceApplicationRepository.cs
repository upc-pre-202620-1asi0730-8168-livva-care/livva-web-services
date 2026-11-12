using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the LifeInsuranceApplication aggregate.
/// </summary>
public interface ILifeInsuranceApplicationRepository : IBaseRepository<LifeInsuranceApplication>
{
    /// <summary>Finds the life details of an application.</summary>
    /// <param name="applicationId">The identifier of the application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The life details if found; otherwise, null.</returns>
    Task<LifeInsuranceApplication?> FindByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default);
}
