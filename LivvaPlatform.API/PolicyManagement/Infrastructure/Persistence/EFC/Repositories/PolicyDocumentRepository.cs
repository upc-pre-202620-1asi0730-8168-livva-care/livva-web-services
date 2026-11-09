using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.PolicyManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for PolicyDocument persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class PolicyDocumentRepository(AppDbContext context) : BaseRepository<PolicyDocument>(context), IPolicyDocumentRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<PolicyDocument>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default) =>
        await Context.Set<PolicyDocument>().Where(d => d.PolicyId == policyId).ToListAsync(cancellationToken);
}
