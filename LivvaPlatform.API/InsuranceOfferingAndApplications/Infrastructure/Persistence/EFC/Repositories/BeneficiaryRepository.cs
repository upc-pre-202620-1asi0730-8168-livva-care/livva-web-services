using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for Beneficiary persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class BeneficiaryRepository(AppDbContext context) : BaseRepository<Beneficiary>(context), IBeneficiaryRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Beneficiary>> FindByPolicyIdAsync(long policyId, CancellationToken cancellationToken = default) =>
        await Context.Set<Beneficiary>().Where(b => b.PolicyId == policyId).ToListAsync(cancellationToken);
}
