using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;

/// <summary>
///     Handles the queries of the User aggregate.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    ///     Gets all users.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The users.</returns>
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a user by its identifier.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a user by its email address.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken = default);
}