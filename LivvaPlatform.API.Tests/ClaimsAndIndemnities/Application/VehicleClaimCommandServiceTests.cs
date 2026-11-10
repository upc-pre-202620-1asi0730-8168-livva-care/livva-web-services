
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.CommandServices;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LivvaPlatform.API.Tests.ClaimsAndIndemnities.Application;

public class VehicleClaimCommandServiceTests
{
    private readonly Mock<IVehicleClaimRepository> _claimRepository = new();
    private readonly Mock<IPolicyRepository> _policyRepository = new();
    private readonly Mock<IIamContextFacade> _iamFacade = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILogger<VehicleClaimCommandService>> _logger = new();

    private VehicleClaimCommandService CreateService()
    {
        return new VehicleClaimCommandService(
            _claimRepository.Object,
            _policyRepository.Object,
            _iamFacade.Object,
            _unitOfWork.Object,
            _logger.Object);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnUserNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new CreateVehicleClaimCommand(
            1, 10, "CLAIM-001", DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Accidente de prueba", 1000m);

        _iamFacade.Setup(x => x.UserExistsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<VehicleClaim, VehicleClaimError>.Failure>(result);
        _claimRepository.Verify(x => x.AddAsync(
            It.IsAny<VehicleClaim>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnPolicyNotFound_WhenPolicyDoesNotExist()
    {
        // Arrange
        var command = new CreateVehicleClaimCommand(
            1, 10, "CLAIM-001", DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Accidente de prueba", 1000m);

        _iamFacade.Setup(x => x.UserExistsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _policyRepository.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Policy?)null);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<VehicleClaim, VehicleClaimError>.Failure>(result);
        _claimRepository.Verify(x => x.AddAsync(
            It.IsAny<VehicleClaim>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateClaim_ShouldSaveClaim_WhenUserAndPolicyExist()
    {
        // Arrange
        var command = new CreateVehicleClaimCommand(
            1, 10, "CLAIM-001", DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Accidente de prueba", 1000m);

        _iamFacade.Setup(x => x.UserExistsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var policy = new Mock<Policy>().Object;

        _policyRepository.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(policy);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<VehicleClaim, VehicleClaimError>.Success>(result);
        _claimRepository.Verify(x => x.AddAsync(
            It.IsAny<VehicleClaim>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeStatus_ShouldReturnNotFound_WhenClaimDoesNotExist()
    {
        // Arrange
        var command = new ChangeVehicleClaimStatusCommand(
            99, VehicleClaimStatus.UNDER_REVIEW, "Revisión iniciada");

        _claimRepository.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleClaim?)null);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<VehicleClaim, VehicleClaimError>.Failure>(result);
        _claimRepository.Verify(x => x.Update(It.IsAny<VehicleClaim>()), Times.Never);
        _unitOfWork.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_ShouldUpdateClaim_WhenClaimExists()
    {
        // Arrange
        var createCommand = new CreateVehicleClaimCommand(
            1, 10, "CLAIM-002", DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Accidente de prueba", 1000m);

        var claim = new VehicleClaim(createCommand);

        var command = new ChangeVehicleClaimStatusCommand(
            claim.Id, VehicleClaimStatus.UNDER_REVIEW, "Revisión iniciada");

        _claimRepository.Setup(x => x.FindByIdAsync(
                claim.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<VehicleClaim, VehicleClaimError>.Success>(result);
        _claimRepository.Verify(x => x.Update(claim), Times.Once);
        _unitOfWork.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
