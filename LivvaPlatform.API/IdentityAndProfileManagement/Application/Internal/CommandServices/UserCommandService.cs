using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service that handles sign-up, sign-in and user update commands.
/// </summary>
/// <param name="userRepository">Repository for accessing user data.</param>
/// <param name="hashingService">Service used to hash and verify passwords.</param>
/// <param name="tokenService">Service used to issue access tokens.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<UserCommandService> logger) : IUserCommandService
{
    /// <inheritdoc />
    public async Task<Result<User, SignUpError>> Handle(SignUpCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = User.NormalizeEmail(command.Email);
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            logger.LogWarning("Sign-up rejected because the email is already registered");
            return new Result<User, SignUpError>.Failure(SignUpError.EmailAlreadyRegistered);
        }

        try
        {
            var user = new User(command, hashingService.HashPassword(command.Password));
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, SignUpError>.Success(user);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex, "Duplicate key violation while registering a user");
            return new Result<User, SignUpError>.Failure(SignUpError.EmailAlreadyRegistered);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            logger.LogError(ex, "Unexpected error while registering a user");
            return new Result<User, SignUpError>.Failure(SignUpError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUser, SignInError>> Handle(SignInCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByEmailAsync(User.NormalizeEmail(command.Email), cancellationToken);
        if (user is null || !user.Active || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
        {
            logger.LogWarning("Sign-in rejected because of invalid credentials");
            return new Result<AuthenticatedUser, SignInError>.Failure(SignInError.InvalidCredentials);
        }

        var token = tokenService.GenerateToken(user);
        return new Result<AuthenticatedUser, SignInError>.Success(new AuthenticatedUser(user, token));
    }

    /// <inheritdoc />
    public async Task<Result<User, UpdateUserError>> Handle(UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user is null) return new Result<User, UpdateUserError>.Failure(UpdateUserError.UserNotFound);

        try
        {
            user.UpdateProfile(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while updating user {UserId}", command.UserId);
            return new Result<User, UpdateUserError>.Failure(UpdateUserError.InvalidData);
        }

        userRepository.Update(user);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<User, UpdateUserError>.Success(user);
    }

    /// <summary>
    ///     Determines whether a DbUpdateException represents a MySQL duplicate key violation (code 1062).
    /// </summary>
    /// <param name="exception">The exception to inspect.</param>
    /// <returns>True if the exception is due to a duplicate key; otherwise, false.</returns>
    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.GetValue(current) is int errorCode && errorCode == 1062) return true;
        }

        return false;
    }
}