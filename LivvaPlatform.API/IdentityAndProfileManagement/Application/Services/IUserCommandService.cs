using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;

/// <summary>
///     Handles the commands of the User aggregate.
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <param name="command">The sign-up command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created user or the error that prevented its creation.</returns>
    Task<Result<User, SignUpError>> Handle(SignUpCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Authenticates a user and issues an access token.
    /// </summary>
    /// <param name="command">The sign-in command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The authenticated user with its token, or an error.</returns>
    Task<Result<AuthenticatedUser, SignInError>> Handle(SignInCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Partially updates the personal data of a user.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated user or an error.</returns>
    Task<Result<User, UpdateUserError>> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default);
}