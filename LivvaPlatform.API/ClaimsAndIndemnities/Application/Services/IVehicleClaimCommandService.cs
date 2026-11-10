using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles the commands of the VehicleClaim aggregate.
/// </summary>
public interface IVehicleClaimCommandService
{
    /// <summary>Registers a vehicle claim.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created vehicle claim or an error.</returns>
    Task<Result<VehicleClaim, VehicleClaimError>> Handle(
        CreateVehicleClaimCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Changes the status of a vehicle claim.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated vehicle claim or an error.</returns>
    Task<Result<VehicleClaim, VehicleClaimError>> Handle(
        ChangeVehicleClaimStatusCommand command,
        CancellationToken cancellationToken = default);
}