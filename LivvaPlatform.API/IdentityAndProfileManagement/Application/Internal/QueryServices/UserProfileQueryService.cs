using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying user profiles.
/// </summary>
/// <param name="userProfileRepository">Repository for accessing profile data.</param>
public class UserProfileQueryService(IUserProfileRepository userProfileRepository) : IUserProfileQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<UserProfile>> Handle(GetAllUserProfilesQuery query,
        CancellationToken cancellationToken = default) =>
        await userProfileRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<UserProfile?> Handle(GetUserProfileByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await userProfileRepository.FindByIdAsync(query.UserProfileId, cancellationToken);

    /// <inheritdoc />
    public async Task<UserProfile?> Handle(GetUserProfileByUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        await userProfileRepository.FindByUserIdAsync(query.UserId, cancellationToken);
}