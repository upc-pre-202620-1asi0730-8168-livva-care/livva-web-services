using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the InsuranceProduct aggregate.
/// </summary>
public interface IInsuranceProductRepository : IBaseRepository<InsuranceProduct>
{
    /// <summary>Finds products filtered by type and insurer.</summary>
    /// <param name="insuranceType">Optional insurance type.</param>
    /// <param name="insurerId">Optional insurer identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching products.</returns>
    Task<IEnumerable<InsuranceProduct>> FindByFiltersAsync(InsuranceType? insuranceType, long? insurerId, CancellationToken cancellationToken = default);
}
