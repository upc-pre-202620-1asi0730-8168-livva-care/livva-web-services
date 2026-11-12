using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the VehicleInsuranceApplication aggregate.
/// </summary>
public interface IVehicleInsuranceApplicationRepository : IBaseRepository<VehicleInsuranceApplication>
{
    /// <summary>Finds the vehicle details of an application.</summary>
    /// <param name="applicationId">The identifier of the application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicle details if found; otherwise, null.</returns>
    Task<VehicleInsuranceApplication?> FindByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether a vehicle is used by an application.</summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the vehicle is used.</returns>
    Task<bool> ExistsByVehicleIdAsync(long vehicleId, CancellationToken cancellationToken = default);
}
