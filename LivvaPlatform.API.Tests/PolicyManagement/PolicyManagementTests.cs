using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LivvaPlatform.API.Tests.PolicyManagement;

/// <summary>
///     Unit tests for the Policy Management bounded context.
/// </summary>
public class PolicyManagementTests
{
    private static CreatePolicyCommand PolicyCommand() => new(1, 1, " liv-veh-9999 ", InsuranceType.VEHICLE,
        new DateOnly(2026, 1, 15), new DateOnly(2027, 1, 14), 1200, 60000);

    private readonly Mock<IPolicyRepository> _policies = new();
    private readonly Mock<IRenewalRepository> _renewals = new();
    private readonly Mock<IIamContextFacade> _iam = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private PolicyCommandService Service() => new(_policies.Object, _renewals.Object, _iam.Object, _unitOfWork.Object,
        NullLogger<PolicyCommandService>.Instance);

    [Fact]
    public void Policy_ShouldStartActiveWithNormalizedNumber()
    {
        var policy = new Policy(PolicyCommand());

        Assert.Equal("LIV-VEH-9999", policy.PolicyNumber);
        Assert.Equal(PolicyStatus.ACTIVE, policy.Status);
        Assert.True(policy.CanBeRenewed);
    }

    [Fact]
    public void Policy_WithExpirationBeforeStart_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Policy(PolicyCommand() with { ExpirationDate = new DateOnly(2025, 1, 1) }));
    }

    [Fact]
    public void RequestRenewal_ForPolicyPendingRenewal_ShouldThrow()
    {
        var policy = new Policy(PolicyCommand());
        policy.MarkAsExpiring();

        Assert.Throws<InvalidOperationException>(() => policy.RequestRenewal());
    }

    [Fact]
    public void Renewal_ShouldStartPendingAndResolve()
    {
        var renewal = new Renewal(new CreateRenewalCommand(1));

        Assert.True(renewal.IsOpen);
        renewal.StartReview();
        renewal.Approve(new DateOnly(2028, 1, 14), 1300);
        Assert.Equal(RenewalStatus.APPROVED, renewal.Status);
        Assert.NotNull(renewal.ResolvedAt);
        Assert.Throws<InvalidOperationException>(() => renewal.Reject());
    }

    [Fact]
    public async Task CreateRenewal_WithOpenRenewal_ShouldFail()
    {
        var policy = new Policy(PolicyCommand());
        _policies.Setup(p => p.FindByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(policy);
        _renewals.Setup(r => r.FindByPolicyIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Renewal(new CreateRenewalCommand(1))]);

        var result = await Service().Handle(new CreateRenewalCommand(1));

        Assert.Equal(PolicyError.RenewalAlreadyRequested, Assert.IsType<Result<Renewal, PolicyError>.Failure>(result).Error);
    }

    [Fact]
    public async Task CreatePolicy_ForUnknownUser_ShouldFail()
    {
        var result = await Service().Handle(PolicyCommand());

        Assert.Equal(PolicyError.UserNotFound, Assert.IsType<Result<Policy, PolicyError>.Failure>(result).Error);
    }
}