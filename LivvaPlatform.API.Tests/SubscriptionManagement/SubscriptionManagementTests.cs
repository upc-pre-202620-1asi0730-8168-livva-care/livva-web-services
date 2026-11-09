using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Application.Internal.CommandServices;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.ValueObjects;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LivvaPlatform.API.Tests.SubscriptionManagement;

/// <summary>
///     Unit tests for the Subscription Management bounded context.
/// </summary>
public class SubscriptionManagementTests
{
    private readonly Mock<ISubscriptionPlanRepository> _plans = new();
    private readonly Mock<IUserSubscriptionRepository> _subscriptions = new();
    private readonly Mock<ISubscriptionPaymentRepository> _payments = new();
    private readonly Mock<IIamContextFacade> _iam = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private SubscriptionCommandService Service() => new(_plans.Object, _subscriptions.Object, _payments.Object,
        _iam.Object, _unitOfWork.Object, NullLogger<SubscriptionCommandService>.Instance);

    [Theory]
    [InlineData(BillingCycle.MONTHLY, 1)]
    [InlineData(BillingCycle.ANNUAL, 12)]
    public void UserSubscription_ShouldStartActiveWithNextBillingDate(BillingCycle cycle, int months)
    {
        var subscription = new UserSubscription(new CreateUserSubscriptionCommand(1, 1, cycle));

        Assert.Equal(SubscriptionStatus.ACTIVE, subscription.Status);
        Assert.Equal(subscription.StartDate.AddMonths(months), subscription.NextBillingDate);
    }

    [Fact]
    public void UserSubscription_Cancel_ShouldSetDatesAndRejectSecondCancel()
    {
        var subscription = new UserSubscription(new CreateUserSubscriptionCommand(1, 1, BillingCycle.MONTHLY));

        subscription.Cancel();

        Assert.Equal(SubscriptionStatus.CANCELLED, subscription.Status);
        Assert.NotNull(subscription.CancelledAt);
        Assert.Throws<InvalidOperationException>(() => subscription.Cancel());
    }

    [Fact]
    public void SubscriptionPayment_Approved_ShouldSetPaidAtAndNormalizeCurrency()
    {
        var payment = new SubscriptionPayment(new CreateSubscriptionPaymentCommand(1, 1, 1, "mercado-pago-sandbox",
            "PAY01", 19, "pen", PaymentStatus.APPROVED, null));

        Assert.Equal("PEN", payment.Currency);
        Assert.NotNull(payment.PaidAt);
    }

    [Fact]
    public async Task CreateSubscription_WhenUserHasActiveSubscription_ShouldFail()
    {
        _iam.Setup(i => i.UserExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var plan = (SubscriptionPlan)Activator.CreateInstance(typeof(SubscriptionPlan), true)!;
        plan.Activate();
        _plans.Setup(p => p.FindByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(plan);
        _subscriptions.Setup(s => s.ExistsActiveByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await Service().Handle(new CreateUserSubscriptionCommand(1, 1, BillingCycle.MONTHLY));

        Assert.Equal(SubscriptionError.ActiveSubscriptionExists,
            Assert.IsType<Result<UserSubscription, SubscriptionError>.Failure>(result).Error);
    }

    [Fact]
    public async Task CreatePayment_ForMissingSubscription_ShouldFail()
    {
        var result = await Service().Handle(new CreateSubscriptionPaymentCommand(9, 1, 1, "mercado-pago-sandbox", null,
            19, "PEN", PaymentStatus.APPROVED, null));

        Assert.Equal(SubscriptionError.SubscriptionNotFound,
            Assert.IsType<Result<SubscriptionPayment, SubscriptionError>.Failure>(result).Error);
    }
}