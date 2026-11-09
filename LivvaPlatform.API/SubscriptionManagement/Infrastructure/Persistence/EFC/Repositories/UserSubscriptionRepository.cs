using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for UserSubscription persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UserSubscriptionRepository(AppDbContext context) : BaseRepository<UserSubscription>(context), IUserSubscriptionRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<UserSubscription>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<UserSubscription>().Where(s => s.UserId == userId).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<UserSubscription>().AnyAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.ACTIVE, cancellationToken);
}
