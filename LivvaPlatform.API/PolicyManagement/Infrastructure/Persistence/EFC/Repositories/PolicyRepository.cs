using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for Policy persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class PolicyRepository(AppDbContext context) : BaseRepository<Policy>(context), IPolicyRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Policy>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<Policy>().Where(p => p.UserId == userId).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default) =>
        await Context.Set<Policy>().AnyAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
}
