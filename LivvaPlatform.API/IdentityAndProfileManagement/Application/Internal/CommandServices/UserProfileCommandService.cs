using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service that handles the creation and update of user profiles.
/// </summary>
/// <param name="userProfileRepository">Repository for accessing profile data.</param>
/// <param name="userRepository">Repository used to verify that the owner user exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class UserProfileCommandService(
    IUserProfileRepository userProfileRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserProfileCommandService> logger) : IUserProfileCommandService
{
    /// <inheritdoc />
    public async Task<Result<UserProfile, UserProfileError>> Handle(CreateUserProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await userRepository.FindByIdAsync(command.UserId, cancellationToken) is null)
            return new Result<UserProfile, UserProfileError>.Failure(UserProfileError.UserNotFound);

        if (await userProfileRepository.FindByUserIdAsync(command.UserId, cancellationToken) is not null)
            return new Result<UserProfile, UserProfileError>.Failure(UserProfileError.ProfileAlreadyExists);

        UserProfile profile;
        try
        {
            profile = new UserProfile(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while creating the profile of user {UserId}", command.UserId);
            return new Result<UserProfile, UserProfileError>.Failure(UserProfileError.InvalidData);
        }

        await userProfileRepository.AddAsync(profile, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<UserProfile, UserProfileError>.Success(profile);
    }

    /// <inheritdoc />
    public async Task<Result<UserProfile, UserProfileError>> Handle(UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await userProfileRepository.FindByIdAsync(command.UserProfileId, cancellationToken);
        if (profile is null)
            return new Result<UserProfile, UserProfileError>.Failure(UserProfileError.ProfileNotFound);

        try
        {
            profile.UpdatePersonalData(command);
            profile.UpdateAddress(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while updating profile {UserProfileId}", command.UserProfileId);
            return new Result<UserProfile, UserProfileError>.Failure(UserProfileError.InvalidData);
        }

        userProfileRepository.Update(profile);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<UserProfile, UserProfileError>.Success(profile);
    }
}