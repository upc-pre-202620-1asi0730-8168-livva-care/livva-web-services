using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for InsuranceProduct persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class InsuranceProductRepository(AppDbContext context) : BaseRepository<InsuranceProduct>(context), IInsuranceProductRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<InsuranceProduct>> FindByFiltersAsync(InsuranceType? insuranceType, long? insurerId, CancellationToken cancellationToken = default) =>
        await Context.Set<InsuranceProduct>()
            .Where(p => insuranceType == null || p.InsuranceType == insuranceType)
            .Where(p => insurerId == null || p.InsurerId == insurerId)
            .ToListAsync(cancellationToken);
}
