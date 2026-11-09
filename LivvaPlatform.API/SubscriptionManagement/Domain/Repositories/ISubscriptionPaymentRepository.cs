using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the SubscriptionPayment aggregate.
/// </summary>
public interface ISubscriptionPaymentRepository : IBaseRepository<SubscriptionPayment>
{
    /// <summary>Finds payments filtered by user and subscription.</summary>
    /// <param name="userId">Optional identifier of the user.</param>
    /// <param name="userSubscriptionId">Optional identifier of the subscription.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The payments.</returns>
    Task<IEnumerable<SubscriptionPayment>> FindByFiltersAsync(long? userId, long? userSubscriptionId, CancellationToken cancellationToken = default);
}
