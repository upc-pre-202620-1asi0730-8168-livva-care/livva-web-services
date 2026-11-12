using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.CommandServices;

/// <summary>
///     Application service that registers vehicle claims and handles status changes.
/// </summary>
/// <param name="vehicleClaimRepository">Repository for accessing vehicle claims.</param>
/// <param name="policyManagementContextFacade">ACL facade to verify that the policy exists.</param>
/// <param name="iamContextFacade">Facade for verifying users.</param>
/// <param name="unitOfWork">Unit of work for managing transactions.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class VehicleClaimCommandService(
    IVehicleClaimRepository vehicleClaimRepository,
    IPolicyManagementContextFacade policyManagementContextFacade,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<VehicleClaimCommandService> logger) : IVehicleClaimCommandService
{
    /// <inheritdoc />
    public async Task<Result<VehicleClaim, VehicleClaimError>> Handle(
        CreateVehicleClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.UserNotFound);

        if (!await policyManagementContextFacade.PolicyExistsAsync(command.PolicyId, cancellationToken))
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.PolicyNotFound);

        VehicleClaim vehicleClaim;

        try
        {
            vehicleClaim = new VehicleClaim(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while registering vehicle claim for policy {PolicyId}", command.PolicyId);
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.InvalidData);
        }

        await vehicleClaimRepository.AddAsync(vehicleClaim, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<VehicleClaim, VehicleClaimError>.Success(vehicleClaim);
    }

    /// <inheritdoc />
    public async Task<Result<VehicleClaim, VehicleClaimError>> Handle(
        ChangeVehicleClaimStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var vehicleClaim = await vehicleClaimRepository.FindByIdAsync(command.ClaimId, cancellationToken);

        if (vehicleClaim is null)
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.VehicleClaimNotFound);

        try
        {
            vehicleClaim.ChangeStatus(command.Status, command.Comment);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid status change for vehicle claim {ClaimId}", command.ClaimId);
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.InvalidStatusChange);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Vehicle claim {ClaimId} cannot change to status {Status}", command.ClaimId, command.Status);
            return new Result<VehicleClaim, VehicleClaimError>.Failure(VehicleClaimError.InvalidStatusChange);
        }

        vehicleClaimRepository.Update(vehicleClaim);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<VehicleClaim, VehicleClaimError>.Success(vehicleClaim);
    }
}