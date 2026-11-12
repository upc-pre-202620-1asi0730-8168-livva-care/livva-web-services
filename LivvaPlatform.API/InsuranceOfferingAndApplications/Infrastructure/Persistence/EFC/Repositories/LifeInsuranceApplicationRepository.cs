using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for LifeInsuranceApplication persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class LifeInsuranceApplicationRepository(AppDbContext context) : BaseRepository<LifeInsuranceApplication>(context), ILifeInsuranceApplicationRepository
{
    /// <inheritdoc />
    public async Task<LifeInsuranceApplication?> FindByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default) =>
        await Context.Set<LifeInsuranceApplication>().FirstOrDefaultAsync(a => a.ApplicationId == applicationId, cancellationToken);
}
