using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Application.Services;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;

namespace LivvaPlatform.API.SubscriptionManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service that subscribes users to plans, cancels subscriptions and registers payments.
/// </summary>
/// <param name="planRepository">Repository for accessing plans.</param>
/// <param name="subscriptionRepository">Repository for accessing subscriptions.</param>
/// <param name="paymentRepository">Repository for accessing payments.</param>
/// <param name="iamContextFacade">Facade used to verify that the user exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class SubscriptionCommandService(
    ISubscriptionPlanRepository planRepository,
    IUserSubscriptionRepository subscriptionRepository,
    ISubscriptionPaymentRepository paymentRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<SubscriptionCommandService> logger) : ISubscriptionCommandService
{
    /// <inheritdoc />
    public async Task<Result<UserSubscription, SubscriptionError>> Handle(CreateUserSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.UserNotFound);

        var plan = await planRepository.FindByIdAsync(command.PlanId, cancellationToken);
        if (plan is null || !plan.Active)
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.PlanNotAvailable);

        if (await subscriptionRepository.ExistsActiveByUserIdAsync(command.UserId, cancellationToken))
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.ActiveSubscriptionExists);

        var subscription = new UserSubscription(command);
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<UserSubscription, SubscriptionError>.Success(subscription);
    }

    /// <inheritdoc />
    public async Task<Result<UserSubscription, SubscriptionError>> Handle(CancelUserSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId, cancellationToken);
        if (subscription is null)
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.SubscriptionNotFound);

        try
        {
            subscription.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Subscription {SubscriptionId} is not active", command.UserSubscriptionId);
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.SubscriptionNotActive);
        }

        subscriptionRepository.Update(subscription);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<UserSubscription, SubscriptionError>.Success(subscription);
    }

    /// <inheritdoc />
    public async Task<Result<UserSubscription, SubscriptionError>> Handle(DeleteUserSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId, cancellationToken);
        if (subscription is null)
            return new Result<UserSubscription, SubscriptionError>.Failure(SubscriptionError.SubscriptionNotFound);

        foreach (var payment in await paymentRepository.FindByFiltersAsync(null, subscription.Id, cancellationToken))
            paymentRepository.Remove(payment);
        subscriptionRepository.Remove(subscription);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<UserSubscription, SubscriptionError>.Success(subscription);
    }

    /// <inheritdoc />
    public async Task<Result<SubscriptionPayment, SubscriptionError>> Handle(CreateSubscriptionPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.UserSubscriptionId, cancellationToken);
        if (subscription is null)
            return new Result<SubscriptionPayment, SubscriptionError>.Failure(SubscriptionError.SubscriptionNotFound);

        SubscriptionPayment payment;
        try
        {
            payment = new SubscriptionPayment(command with { UserId = subscription.UserId, PlanId = subscription.PlanId });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while registering a payment for subscription {SubscriptionId}", command.UserSubscriptionId);
            return new Result<SubscriptionPayment, SubscriptionError>.Failure(SubscriptionError.InvalidData);
        }

        await paymentRepository.AddAsync(payment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<SubscriptionPayment, SubscriptionError>.Success(payment);
    }
}