
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.QueryServices;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using Moq;
using Xunit;

namespace LivvaPlatform.API.Tests.ClaimsAndIndemnities.Application;

public class VehicleClaimQueryServiceTests
{
    private readonly Mock<IVehicleClaimRepository> _repository = new();

    private VehicleClaimQueryService CreateService() =>
        new(_repository.Object);

    private static VehicleClaim CreateClaim() =>
        new(new CreateVehicleClaimCommand(
            1,
            10,
            "CLAIM-QUERY-001",
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            "Accidente de prueba",
            1000m));

    [Fact]
    public async Task GetById_ShouldReturnClaim_WhenClaimExists()
    {
        var claim = CreateClaim();

        _repository.Setup(x => x.FindByIdAsync(
                1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        var result = await CreateService().Handle(
            new GetVehicleClaimByIdQuery(1));

        Assert.Same(claim, result);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenClaimDoesNotExist()
    {
        _repository.Setup(x => x.FindByIdAsync(
                99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleClaim?)null);

        var result = await CreateService().Handle(
            new GetVehicleClaimByIdQuery(99));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnUserClaims()
    {
        var claims = new[] { CreateClaim() };

        _repository.Setup(x => x.FindByUserIdAsync(10))
            .ReturnsAsync(claims);

        var result = await CreateService().Handle(
            new GetVehicleClaimsByUserIdQuery(10));

        Assert.Same(claims, result);
    }

    [Fact]
    public async Task GetByPolicyId_ShouldReturnPolicyClaims()
    {
        var claims = new[] { CreateClaim() };

        _repository.Setup(x => x.FindByPolicyIdAsync(1))
            .ReturnsAsync(claims);

        var result = await CreateService().Handle(
            new GetVehicleClaimsByPolicyIdQuery(1));

        Assert.Same(claims, result);
    }
}
