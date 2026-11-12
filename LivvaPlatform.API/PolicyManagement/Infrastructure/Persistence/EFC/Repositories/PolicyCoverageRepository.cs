using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for PolicyCoverage persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class PolicyCoverageRepository(AppDbContext context) : BaseRepository<PolicyCoverage>(context), IPolicyCoverageRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<PolicyCoverage>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default) =>
        await Context.Set<PolicyCoverage>().Where(c => c.PolicyId == policyId).ToListAsync(cancellationToken);
}
