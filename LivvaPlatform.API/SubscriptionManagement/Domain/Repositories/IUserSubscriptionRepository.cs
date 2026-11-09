using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the UserSubscription aggregate.
/// </summary>
public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription>
{
    /// <summary>Finds the subscriptions of a user.</summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The subscriptions.</returns>
    Task<IEnumerable<UserSubscription>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether a user has an active subscription.</summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the user has an active subscription.</returns>
    Task<bool> ExistsActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
