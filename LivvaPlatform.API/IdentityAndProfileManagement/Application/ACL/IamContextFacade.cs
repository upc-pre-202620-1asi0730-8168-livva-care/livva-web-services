using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.ACL;

/// <summary>
///     Implementation of the Identity and Profile Management anti-corruption layer facade.
/// </summary>
/// <param name="userRepository">Repository for accessing user data.</param>
public class IamContextFacade(IUserRepository userRepository) : IIamContextFacade
{
    /// <inheritdoc />
    public async Task<bool> UserExistsAsync(long userId, CancellationToken cancellationToken = default) =>
        await userRepository.FindByIdAsync(userId, cancellationToken) is not null;
}