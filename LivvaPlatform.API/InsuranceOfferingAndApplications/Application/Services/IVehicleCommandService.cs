using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the commands of the Vehicle aggregate.
/// </summary>
public interface IVehicleCommandService
{
    /// <summary>Registers a vehicle.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created vehicle or an error.</returns>
    Task<Result<Vehicle, VehicleError>> Handle(CreateVehicleCommand command, CancellationToken cancellationToken = default);

    /// <summary>Partially updates a vehicle.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated vehicle or an error.</returns>
    Task<Result<Vehicle, VehicleError>> Handle(UpdateVehicleCommand command, CancellationToken cancellationToken = default);

    /// <summary>Deletes a vehicle that is not used by an insurance application.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The deleted vehicle or an error.</returns>
    Task<Result<Vehicle, VehicleError>> Handle(DeleteVehicleCommand command, CancellationToken cancellationToken = default);
}