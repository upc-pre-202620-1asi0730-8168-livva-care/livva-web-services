using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.SubscriptionManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for SubscriptionPlan persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SubscriptionPlanRepository(AppDbContext context) : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SubscriptionPlan>> FindActiveAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<SubscriptionPlan>().Where(p => p.Active).ToListAsync(cancellationToken);
}
