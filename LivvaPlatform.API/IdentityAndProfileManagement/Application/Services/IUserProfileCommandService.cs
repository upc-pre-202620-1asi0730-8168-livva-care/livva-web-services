using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;

/// <summary>
///     Handles the commands of the UserProfile aggregate.
/// </summary>
public interface IUserProfileCommandService
{
    /// <summary>
    ///     Creates the profile of a user.
    /// </summary>
    /// <param name="command">The create command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created profile or an error.</returns>
    Task<Result<UserProfile, UserProfileError>> Handle(CreateUserProfileCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Partially updates a user profile.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated profile or an error.</returns>
    Task<Result<UserProfile, UserProfileError>> Handle(UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default);
}