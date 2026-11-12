using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;

/// <summary>
///     Repository contract for the User aggregate.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Finds a user by its email address.
    /// </summary>
    /// <param name="email">The normalized email address.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether a user with the given email address exists.
    /// </summary>
    /// <param name="email">The normalized email address.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if a user exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}