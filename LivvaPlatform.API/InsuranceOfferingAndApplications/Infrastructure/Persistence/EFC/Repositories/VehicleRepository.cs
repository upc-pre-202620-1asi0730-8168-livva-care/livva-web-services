using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for Vehicle persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class VehicleRepository(AppDbContext context) : BaseRepository<Vehicle>(context), IVehicleRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        await Context.Set<Vehicle>().Where(v => v.UserId == userId).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate, long? excludedVehicleId = null, CancellationToken cancellationToken = default) =>
        await Context.Set<Vehicle>().AnyAsync(v => v.LicensePlate == licensePlate && v.Id != excludedVehicleId, cancellationToken);
}
