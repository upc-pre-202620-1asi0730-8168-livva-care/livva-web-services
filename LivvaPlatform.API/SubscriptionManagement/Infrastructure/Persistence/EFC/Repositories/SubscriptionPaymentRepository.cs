using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for SubscriptionPayment persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SubscriptionPaymentRepository(AppDbContext context) : BaseRepository<SubscriptionPayment>(context), ISubscriptionPaymentRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SubscriptionPayment>> FindByFiltersAsync(long? userId, long? userSubscriptionId, CancellationToken cancellationToken = default) =>
        await Context.Set<SubscriptionPayment>()
            .Where(p => userId == null || p.UserId == userId)
            .Where(p => userSubscriptionId == null || p.UserSubscriptionId == userSubscriptionId)
            .ToListAsync(cancellationToken);
}
