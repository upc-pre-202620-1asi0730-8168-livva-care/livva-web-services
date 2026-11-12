using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the SubscriptionPlan aggregate.
/// </summary>
public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan>
{
    /// <summary>Finds the active plans.</summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The active plans.</returns>
    Task<IEnumerable<SubscriptionPlan>> FindActiveAsync(CancellationToken cancellationToken = default);
}
