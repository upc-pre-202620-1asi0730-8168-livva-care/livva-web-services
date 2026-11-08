using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying users.
/// </summary>
/// <param name="userRepository">Repository for accessing user data.</param>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default) =>
        await userRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default) =>
        await userRepository.FindByIdAsync(query.UserId, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken = default) =>
        await userRepository.FindByEmailAsync(User.NormalizeEmail(query.Email), cancellationToken);
}