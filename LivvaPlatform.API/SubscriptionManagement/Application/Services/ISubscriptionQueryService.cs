using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

namespace LivvaPlatform.API.SubscriptionManagement.Application.Services;

/// <summary>
///     Handles the queries of plans, user subscriptions and subscription payments.
/// </summary>
public interface ISubscriptionQueryService
{
    /// <summary>Gets plans.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The plans.</returns>
    Task<IEnumerable<SubscriptionPlan>> Handle(GetSubscriptionPlansQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a plan by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The plan if found; otherwise, null.</returns>
    Task<SubscriptionPlan?> Handle(GetSubscriptionPlanByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets user subscriptions.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The subscriptions.</returns>
    Task<IEnumerable<UserSubscription>> Handle(GetUserSubscriptionsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a user subscription by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The subscription if found; otherwise, null.</returns>
    Task<UserSubscription?> Handle(GetUserSubscriptionByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets subscription payments.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The payments.</returns>
    Task<IEnumerable<SubscriptionPayment>> Handle(GetSubscriptionPaymentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a subscription payment by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The payment if found; otherwise, null.</returns>
    Task<SubscriptionPayment?> Handle(GetSubscriptionPaymentByIdQuery query, CancellationToken cancellationToken = default);
}