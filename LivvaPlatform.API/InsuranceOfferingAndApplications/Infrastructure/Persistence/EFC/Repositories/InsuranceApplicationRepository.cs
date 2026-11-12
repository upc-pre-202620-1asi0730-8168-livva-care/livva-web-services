using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for InsuranceApplication persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class InsuranceApplicationRepository(AppDbContext context) : BaseRepository<InsuranceApplication>(context), IInsuranceApplicationRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<InsuranceApplication>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<InsuranceApplication>().Where(a => a.UserId == userId).ToListAsync(cancellationToken);
}
