using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the UserProfile aggregate.
/// </summary>
public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    /// <summary>
    ///     Finds the profile of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<UserProfile?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}