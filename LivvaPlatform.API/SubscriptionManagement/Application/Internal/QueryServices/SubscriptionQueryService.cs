using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Application.Services;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;
using LivvaPlatform.API.SubscriptionManagement.Domain.Repositories;

namespace LivvaPlatform.API.SubscriptionManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying plans, user subscriptions and payments.
/// </summary>
/// <param name="planRepository">Repository for accessing plans.</param>
/// <param name="subscriptionRepository">Repository for accessing subscriptions.</param>
/// <param name="paymentRepository">Repository for accessing payments.</param>
public class SubscriptionQueryService(
    ISubscriptionPlanRepository planRepository,
    IUserSubscriptionRepository subscriptionRepository,
    ISubscriptionPaymentRepository paymentRepository) : ISubscriptionQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<SubscriptionPlan>> Handle(GetSubscriptionPlansQuery query, CancellationToken cancellationToken = default) =>
        query.Active == true
            ? await planRepository.FindActiveAsync(cancellationToken)
            : (await planRepository.ListAsync(cancellationToken)).Where(p => query.Active is null || p.Active == query.Active);

    /// <inheritdoc />
    public async Task<SubscriptionPlan?> Handle(GetSubscriptionPlanByIdQuery query, CancellationToken cancellationToken = default) =>
        await planRepository.FindByIdAsync(query.PlanId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<UserSubscription>> Handle(GetUserSubscriptionsQuery query, CancellationToken cancellationToken = default) =>
        query.UserId is null
            ? await subscriptionRepository.ListAsync(cancellationToken)
            : await subscriptionRepository.FindByUserIdAsync(query.UserId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<UserSubscription?> Handle(GetUserSubscriptionByIdQuery query, CancellationToken cancellationToken = default) =>
        await subscriptionRepository.FindByIdAsync(query.UserSubscriptionId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<SubscriptionPayment>> Handle(GetSubscriptionPaymentsQuery query, CancellationToken cancellationToken = default) =>
        await paymentRepository.FindByFiltersAsync(query.UserId, query.UserSubscriptionId, cancellationToken);

    /// <inheritdoc />
    public async Task<SubscriptionPayment?> Handle(GetSubscriptionPaymentByIdQuery query, CancellationToken cancellationToken = default) =>
        await paymentRepository.FindByIdAsync(query.SubscriptionPaymentId, cancellationToken);
}