using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;

/// <summary>
///     Handles the queries of the UserProfile aggregate.
/// </summary>
public interface IUserProfileQueryService
{
    /// <summary>
    ///     Gets all user profiles.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The profiles.</returns>
    Task<IEnumerable<UserProfile>> Handle(GetAllUserProfilesQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a user profile by its identifier.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<UserProfile?> Handle(GetUserProfileByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the profile of a user.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<UserProfile?> Handle(GetUserProfileByUserIdQuery query, CancellationToken cancellationToken = default);
}