using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.CommandServices;

/// <summary>
///     Application service that registers, updates and deletes vehicles.
/// </summary>
/// <param name="vehicleRepository">Repository for accessing vehicles.</param>
/// <param name="vehicleApplicationRepository">Repository used to check whether a vehicle is in use.</param>
/// <param name="iamContextFacade">Facade used to verify that the owner exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class VehicleCommandService(
    IVehicleRepository vehicleRepository,
    IVehicleInsuranceApplicationRepository vehicleApplicationRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<VehicleCommandService> logger) : IVehicleCommandService
{
    /// <inheritdoc />
    public async Task<Result<Vehicle, VehicleError>> Handle(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<Vehicle, VehicleError>.Failure(VehicleError.UserNotFound);

        Vehicle vehicle;
        try
        {
            vehicle = new Vehicle(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while registering a vehicle for user {UserId}", command.UserId);
            return new Result<Vehicle, VehicleError>.Failure(VehicleError.InvalidData);
        }

        if (await vehicleRepository.ExistsByLicensePlateAsync(vehicle.LicensePlate, null, cancellationToken))
            return new Result<Vehicle, VehicleError>.Failure(VehicleError.LicensePlateAlreadyRegistered);

        await vehicleRepository.AddAsync(vehicle, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Vehicle, VehicleError>.Success(vehicle);
    }

    /// <inheritdoc />
    public async Task<Result<Vehicle, VehicleError>> Handle(UpdateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        var vehicle = await vehicleRepository.FindByIdAsync(command.VehicleId, cancellationToken);
        if (vehicle is null) return new Result<Vehicle, VehicleError>.Failure(VehicleError.VehicleNotFound);

        try
        {
            if (command.LicensePlate is not null &&
                await vehicleRepository.ExistsByLicensePlateAsync(Vehicle.NormalizeLicensePlate(command.LicensePlate), vehicle.Id, cancellationToken))
                return new Result<Vehicle, VehicleError>.Failure(VehicleError.LicensePlateAlreadyRegistered);
            vehicle.UpdateInformation(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while updating vehicle {VehicleId}", command.VehicleId);
            return new Result<Vehicle, VehicleError>.Failure(VehicleError.InvalidData);
        }

        vehicleRepository.Update(vehicle);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Vehicle, VehicleError>.Success(vehicle);
    }

    /// <inheritdoc />
    public async Task<Result<Vehicle, VehicleError>> Handle(DeleteVehicleCommand command, CancellationToken cancellationToken = default)
    {
        var vehicle = await vehicleRepository.FindByIdAsync(command.VehicleId, cancellationToken);
        if (vehicle is null) return new Result<Vehicle, VehicleError>.Failure(VehicleError.VehicleNotFound);
        if (await vehicleApplicationRepository.ExistsByVehicleIdAsync(vehicle.Id, cancellationToken))
            return new Result<Vehicle, VehicleError>.Failure(VehicleError.VehicleInUse);

        vehicleRepository.Remove(vehicle);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Vehicle, VehicleError>.Success(vehicle);
    }
}