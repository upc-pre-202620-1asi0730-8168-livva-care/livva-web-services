
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.CommandServices;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LivvaPlatform.API.Tests.ClaimsAndIndemnities.Application;

public class IndemnityRequestCommandServiceTests
{
    private readonly Mock<IIndemnityRequestRepository> _requestRepository = new();
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepository = new();
    private readonly Mock<IPolicyManagementContextFacade> _policyFacade = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILogger<IndemnityRequestCommandService>> _logger = new();

    private IndemnityRequestCommandService CreateService()
    {
        return new IndemnityRequestCommandService(
            _requestRepository.Object,
            _beneficiaryRepository.Object,
            _policyFacade.Object,
            _unitOfWork.Object,
            _logger.Object);
    }

    [Fact]
    public async Task CreateRequest_ShouldReturnPolicyNotFound_WhenPolicyDoesNotExist()
    {
        // Arrange
        var command = new CreateIndemnityRequestCommand(
            1, 10, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Solicitud de indemnización", 5000m);

        _policyFacade.Setup(x => x.PolicyExistsAsync(
                1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<IndemnityRequest, IndemnityRequestError>.Failure>(result);

        _beneficiaryRepository.Verify(x => x.FindByPolicyIdAsync(
            It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);

        _requestRepository.Verify(x => x.AddAsync(
            It.IsAny<IndemnityRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWork.Verify(x => x.CompleteAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRequest_ShouldReturnBeneficiaryNotFound_WhenBeneficiaryDoesNotBelongToPolicy()
    {
        // Arrange
        var command = new CreateIndemnityRequestCommand(
            1, 10, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Solicitud de indemnización", 5000m);

        _policyFacade.Setup(x => x.PolicyExistsAsync(
                1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _beneficiaryRepository.Setup(x => x.FindByPolicyIdAsync(
                1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Beneficiary>());

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<IndemnityRequest, IndemnityRequestError>.Failure>(result);

        _requestRepository.Verify(x => x.AddAsync(
            It.IsAny<IndemnityRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWork.Verify(x => x.CompleteAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var command = new ChangeIndemnityRequestStatusCommand(
            99, IndemnityRequestStatus.UNDER_REVIEW, "Revisión iniciada");

        _requestRepository.Setup(x => x.FindByIdAsync(
                99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IndemnityRequest?)null);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<IndemnityRequest, IndemnityRequestError>.Failure>(result);

        _requestRepository.Verify(x => x.Update(
            It.IsAny<IndemnityRequest>()), Times.Never);

        _unitOfWork.Verify(x => x.CompleteAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_ShouldUpdateRequest_WhenRequestExists()
    {
        // Arrange
        var createCommand = new CreateIndemnityRequestCommand(
            1, 10, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Solicitud de indemnización", 5000m);

        var request = new IndemnityRequest(createCommand);

        var command = new ChangeIndemnityRequestStatusCommand(
            1, IndemnityRequestStatus.UNDER_REVIEW, "Revisión iniciada");

        _requestRepository.Setup(x => x.FindByIdAsync(
                1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        // Act
        var result = await CreateService().Handle(command);

        // Assert
        Assert.IsType<Result<IndemnityRequest, IndemnityRequestError>.Success>(result);

        _requestRepository.Verify(x => x.Update(request), Times.Once);

        _unitOfWork.Verify(x => x.CompleteAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
