using LivvaPlatform.API.NotificationManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.NotificationManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.NotificationManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for notification persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class NotificationRepository(AppDbContext context) : BaseRepository<Notification>(context), INotificationRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Notification>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<Notification>().Where(n => n.UserId == userId).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(long userId, string type, long policyId, CancellationToken cancellationToken = default) =>
        await Context.Set<Notification>().AnyAsync(n => n.UserId == userId && n.Type == type && n.PolicyId == policyId, cancellationToken);
}