using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for Renewal persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class RenewalRepository(AppDbContext context) : BaseRepository<Renewal>(context), IRenewalRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Renewal>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default) =>
        await Context.Set<Renewal>().Where(r => r.PolicyId == policyId).ToListAsync(cancellationToken);
}
