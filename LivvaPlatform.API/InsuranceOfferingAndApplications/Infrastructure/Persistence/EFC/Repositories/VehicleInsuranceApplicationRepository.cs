using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for VehicleInsuranceApplication persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class VehicleInsuranceApplicationRepository(AppDbContext context) : BaseRepository<VehicleInsuranceApplication>(context), IVehicleInsuranceApplicationRepository
{
    /// <inheritdoc />
    public async Task<VehicleInsuranceApplication?> FindByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default) =>
        await Context.Set<VehicleInsuranceApplication>().FirstOrDefaultAsync(a => a.ApplicationId == applicationId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsByVehicleIdAsync(long vehicleId, CancellationToken cancellationToken = default) =>
        await Context.Set<VehicleInsuranceApplication>().AnyAsync(a => a.VehicleId == vehicleId, cancellationToken);
}
