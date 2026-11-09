using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.NotificationManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the Notification aggregate.
/// </summary>
public interface INotificationRepository : IBaseRepository<Notification>
{
    /// <summary>Finds the notifications of a user.</summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notifications.</returns>
    Task<IEnumerable<Notification>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether a user already has a notification of a type for a policy.</summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="type">The notification type.</param>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the notification exists.</returns>
    Task<bool> ExistsAsync(long userId, string type, long policyId, CancellationToken cancellationToken = default);
}