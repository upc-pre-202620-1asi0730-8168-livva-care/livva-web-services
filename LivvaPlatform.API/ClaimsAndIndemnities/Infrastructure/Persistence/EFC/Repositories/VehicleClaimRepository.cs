using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for vehicle claim persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class VehicleClaimRepository(AppDbContext context)
    : BaseRepository<VehicleClaim>(context), IVehicleClaimRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> FindByUserIdAsync(long userId) =>
        await Context.Set<VehicleClaim>()
            .Where(claim => claim.UserId == userId)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleClaim>> FindByPolicyIdAsync(long policyId) =>
        await Context.Set<VehicleClaim>()
            .Where(claim => claim.PolicyId == policyId)
            .ToListAsync();
}