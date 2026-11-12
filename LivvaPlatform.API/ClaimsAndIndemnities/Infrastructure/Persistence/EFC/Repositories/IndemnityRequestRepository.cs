using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for indemnity request persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class IndemnityRequestRepository(AppDbContext context)
    : BaseRepository<IndemnityRequest>(context), IIndemnityRequestRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<IndemnityRequest>> FindByBeneficiaryIdAsync(long beneficiaryId) =>
        await Context.Set<IndemnityRequest>()
            .Where(request => request.BeneficiaryId == beneficiaryId)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<IndemnityRequest>> FindByPolicyIdAsync(long policyId) =>
        await Context.Set<IndemnityRequest>()
            .Where(request => request.PolicyId == policyId)
            .ToListAsync();
}