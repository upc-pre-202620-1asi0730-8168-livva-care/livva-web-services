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
///     Application service that submits and deletes insurance applications and registers their vehicle and life details.
/// </summary>
/// <param name="applicationRepository">Repository for accessing applications.</param>
/// <param name="vehicleApplicationRepository">Repository for accessing vehicle details.</param>
/// <param name="lifeApplicationRepository">Repository for accessing life details.</param>
/// <param name="insuranceProductRepository">Repository for accessing insurance products.</param>
/// <param name="vehicleRepository">Repository for accessing vehicles.</param>
/// <param name="iamContextFacade">Facade used to verify that the applicant exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class InsuranceApplicationCommandService(
    IInsuranceApplicationRepository applicationRepository,
    IVehicleInsuranceApplicationRepository vehicleApplicationRepository,
    ILifeInsuranceApplicationRepository lifeApplicationRepository,
    IInsuranceProductRepository insuranceProductRepository,
    IVehicleRepository vehicleRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<InsuranceApplicationCommandService> logger) : IInsuranceApplicationCommandService
{
    /// <inheritdoc />
    public async Task<Result<InsuranceApplication, InsuranceApplicationError>> Handle(CreateInsuranceApplicationCommand command, CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.UserNotFound);

        var product = await insuranceProductRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null || !product.Active)
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.ProductNotAvailable);

        InsuranceApplication application;
        try
        {
            application = new InsuranceApplication(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while creating an insurance application for user {UserId}", command.UserId);
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.InvalidData);
        }

        await applicationRepository.AddAsync(application, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InsuranceApplication, InsuranceApplicationError>.Success(application);
    }

    /// <inheritdoc />
    public async Task<Result<InsuranceApplication, InsuranceApplicationError>> Handle(ChangeInsuranceApplicationStatusCommand command, CancellationToken cancellationToken = default)
    {
        var application = await applicationRepository.FindByIdAsync(command.ApplicationId, cancellationToken);
        if (application is null)
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.ApplicationNotFound);

        try
        {
            switch (command.Status)
            {
                case RequestStatus.UNDER_REVIEW: application.StartEvaluation(); break;
                case RequestStatus.APPROVED: application.Approve(); break;
                case RequestStatus.REJECTED: application.Reject(); break;
                case RequestStatus.CANCELLED: application.Cancel(); break;
                default:
                    return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.InvalidStatusChange);
            }
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid status change for insurance application {ApplicationId}", command.ApplicationId);
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.InvalidStatusChange);
        }

        applicationRepository.Update(application);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InsuranceApplication, InsuranceApplicationError>.Success(application);
    }

    /// <inheritdoc />
    public async Task<Result<InsuranceApplication, InsuranceApplicationError>> Handle(DeleteInsuranceApplicationCommand command, CancellationToken cancellationToken = default)
    {
        var application = await applicationRepository.FindByIdAsync(command.ApplicationId, cancellationToken);
        if (application is null)
            return new Result<InsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.ApplicationNotFound);

        var vehicleDetails = await vehicleApplicationRepository.FindByApplicationIdAsync(application.Id, cancellationToken);
        if (vehicleDetails is not null) vehicleApplicationRepository.Remove(vehicleDetails);
        var lifeDetails = await lifeApplicationRepository.FindByApplicationIdAsync(application.Id, cancellationToken);
        if (lifeDetails is not null) lifeApplicationRepository.Remove(lifeDetails);

        applicationRepository.Remove(application);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InsuranceApplication, InsuranceApplicationError>.Success(application);
    }

    /// <inheritdoc />
    public async Task<Result<VehicleInsuranceApplication, InsuranceApplicationError>> Handle(CreateVehicleInsuranceApplicationCommand command, CancellationToken cancellationToken = default)
    {
        var check = await ValidateLinkAsync(command.ApplicationId, InsuranceType.VEHICLE, cancellationToken);
        if (check is not null) return new Result<VehicleInsuranceApplication, InsuranceApplicationError>.Failure(check.Value.Error);

        var application = (await applicationRepository.FindByIdAsync(command.ApplicationId, cancellationToken))!;
        var vehicle = await vehicleRepository.FindByIdAsync(command.VehicleId, cancellationToken);
        if (vehicle is null || vehicle.UserId != application.UserId)
            return new Result<VehicleInsuranceApplication, InsuranceApplicationError>.Failure(InsuranceApplicationError.VehicleNotFound);

        var details = new VehicleInsuranceApplication(command);
        await vehicleApplicationRepository.AddAsync(details, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<VehicleInsuranceApplication, InsuranceApplicationError>.Success(details);
    }

    /// <inheritdoc />
    public async Task<Result<LifeInsuranceApplication, InsuranceApplicationError>> Handle(CreateLifeInsuranceApplicationCommand command, CancellationToken cancellationToken = default)
    {
        var check = await ValidateLinkAsync(command.ApplicationId, InsuranceType.LIFE, cancellationToken);
        if (check is not null) return new Result<LifeInsuranceApplication, InsuranceApplicationError>.Failure(check.Value.Error);

        var details = new LifeInsuranceApplication(command);
        await lifeApplicationRepository.AddAsync(details, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<LifeInsuranceApplication, InsuranceApplicationError>.Success(details);
    }

    /// <summary>
    ///     Verifies that an application exists, has no details yet and was submitted for a product of the expected type.
    /// </summary>
    private async Task<(InsuranceApplicationError Error, bool Failed)?> ValidateLinkAsync(long applicationId, InsuranceType expectedType, CancellationToken cancellationToken)
    {
        var application = await applicationRepository.FindByIdAsync(applicationId, cancellationToken);
        if (application is null) return (InsuranceApplicationError.ApplicationNotFound, true);

        if (await vehicleApplicationRepository.FindByApplicationIdAsync(applicationId, cancellationToken) is not null ||
            await lifeApplicationRepository.FindByApplicationIdAsync(applicationId, cancellationToken) is not null)
            return (InsuranceApplicationError.ApplicationAlreadyLinked, true);

        var product = await insuranceProductRepository.FindByIdAsync(application.ProductId, cancellationToken);
        if (product is null || product.InsuranceType != expectedType)
            return (InsuranceApplicationError.ProductTypeMismatch, true);

        return null;
    }
}