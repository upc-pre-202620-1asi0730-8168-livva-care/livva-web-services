using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework implementation of <see cref="IVehicleClaimRepository" />.
/// </summary>
public class VehicleClaimRepository(AppDbContext context)
    : BaseRepository<VehicleClaim>(context), IVehicleClaimRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> FindByUserIdAsync(long userId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<VehicleClaim>()
            .Where(claim => claim.UserId == userId)
            .OrderByDescending(claim => claim.Id)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> FindByPolicyIdAsync(long policyId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<VehicleClaim>()
            .Where(claim => claim.PolicyId == policyId)
            .OrderByDescending(claim => claim.Id)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<ClaimStatusHistory>> FindStatusHistoriesByClaimIdAsync(long claimId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<ClaimStatusHistory>()
            .Where(history => history.ClaimId == claimId)
            .OrderByDescending(history => history.Id)
            .ToListAsync(cancellationToken);
}
