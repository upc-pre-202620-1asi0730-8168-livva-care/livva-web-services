using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the Vehicle aggregate.
/// </summary>
public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    /// <summary>Finds the vehicles of a user.</summary>
    /// <param name="userId">The identifier of the owner.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicles.</returns>
    Task<IEnumerable<Vehicle>> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether another vehicle uses the license plate.</summary>
    /// <param name="licensePlate">The normalized license plate.</param>
    /// <param name="excludedVehicleId">A vehicle to exclude from the check.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the license plate is in use.</returns>
    Task<bool> ExistsByLicensePlateAsync(string licensePlate, long? excludedVehicleId = null, CancellationToken cancellationToken = default);
}
