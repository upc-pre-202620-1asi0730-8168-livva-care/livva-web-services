namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;

/// <summary>
///     Anti-corruption layer facade that exposes Identity and Profile Management data to other bounded contexts.
/// </summary>
public interface IIamContextFacade
{
    /// <summary>
    ///     Determines whether a user exists.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> UserExistsAsync(long userId, CancellationToken cancellationToken = default);
}