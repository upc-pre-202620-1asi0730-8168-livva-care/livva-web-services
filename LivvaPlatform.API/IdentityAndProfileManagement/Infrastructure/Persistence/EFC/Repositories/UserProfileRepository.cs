using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for user profile persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UserProfileRepository(AppDbContext context)
    : BaseRepository<UserProfile>(context), IUserProfileRepository
{
    /// <inheritdoc />
    public async Task<UserProfile?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<UserProfile>().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
}