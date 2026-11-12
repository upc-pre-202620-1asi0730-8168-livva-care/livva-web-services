using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.CommandServices;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LivvaPlatform.API.Tests.InsuranceOfferingAndApplications.Application;

/// <summary>
///     Unit tests for the command services of the Insurance Offering and Applications bounded context.
/// </summary>
public class CommandServicesTests
{
    private readonly Mock<IVehicleRepository> _vehicles = new();
    private readonly Mock<IVehicleInsuranceApplicationRepository> _vehicleApplications = new();
    private readonly Mock<ILifeInsuranceApplicationRepository> _lifeApplications = new();
    private readonly Mock<IInsuranceApplicationRepository> _applications = new();
    private readonly Mock<IInsuranceProductRepository> _products = new();
    private readonly Mock<IIamContextFacade> _iam = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private VehicleCommandService VehicleService() => new(_vehicles.Object, _vehicleApplications.Object, _iam.Object,
        _unitOfWork.Object, NullLogger<VehicleCommandService>.Instance);

    private InsuranceApplicationCommandService ApplicationService() => new(_applications.Object,
        _vehicleApplications.Object, _lifeApplications.Object, _products.Object, _vehicles.Object, _iam.Object,
        _unitOfWork.Object, NullLogger<InsuranceApplicationCommandService>.Instance);

    [Fact]
    public async Task CreateVehicle_ForUnknownUser_ShouldFail()
    {
        var result = await VehicleService().Handle(new CreateVehicleCommand(9, "ABC-123", "Toyota", "Corolla", 2022, 65000));

        Assert.Equal(VehicleError.UserNotFound, Assert.IsType<Result<Vehicle, VehicleError>.Failure>(result).Error);
    }

    [Fact]
    public async Task CreateVehicle_WithDuplicatePlate_ShouldFail()
    {
        _iam.Setup(i => i.UserExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _vehicles.Setup(v => v.ExistsByLicensePlateAsync("ABC-123", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await VehicleService().Handle(new CreateVehicleCommand(1, "abc-123", "Toyota", "Corolla", 2022, 65000));

        Assert.Equal(VehicleError.LicensePlateAlreadyRegistered,
            Assert.IsType<Result<Vehicle, VehicleError>.Failure>(result).Error);
        _vehicles.Verify(v => v.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteVehicle_InUse_ShouldFail()
    {
        var vehicle = new Vehicle(new CreateVehicleCommand(1, "ABC-123", "Toyota", "Corolla", 2022, 65000));
        _vehicles.Setup(v => v.FindByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
        _vehicleApplications.Setup(a => a.ExistsByVehicleIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await VehicleService().Handle(new DeleteVehicleCommand(5));

        Assert.Equal(VehicleError.VehicleInUse, Assert.IsType<Result<Vehicle, VehicleError>.Failure>(result).Error);
    }

    [Fact]
    public async Task CreateApplication_ForUnavailableProduct_ShouldFail()
    {
        _iam.Setup(i => i.UserExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await ApplicationService().Handle(new CreateInsuranceApplicationCommand(1, 99, 1000));

        Assert.Equal(InsuranceApplicationError.ProductNotAvailable,
            Assert.IsType<Result<InsuranceApplication, InsuranceApplicationError>.Failure>(result).Error);
    }

    [Fact]
    public async Task CreateLifeDetails_ForMissingApplication_ShouldFail()
    {
        var result = await ApplicationService().Handle(new CreateLifeInsuranceApplicationCommand(42));

        Assert.Equal(InsuranceApplicationError.ApplicationNotFound,
            Assert.IsType<Result<LifeInsuranceApplication, InsuranceApplicationError>.Failure>(result).Error);
    }
}