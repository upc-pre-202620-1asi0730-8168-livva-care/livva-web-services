using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.SubscriptionManagement.Application.Errors;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Commands;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Queries;

namespace LivvaPlatform.API.SubscriptionManagement.Application.Services;

/// <summary>
///     Handles the commands of user subscriptions and subscription payments.
/// </summary>
public interface ISubscriptionCommandService
{
    /// <summary>Subscribes a user to a plan.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created subscription or an error.</returns>
    Task<Result<UserSubscription, SubscriptionError>> Handle(CreateUserSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Cancels an active subscription.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The cancelled subscription or an error.</returns>
    Task<Result<UserSubscription, SubscriptionError>> Handle(CancelUserSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Deletes a subscription and its payments.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The deleted subscription or an error.</returns>
    Task<Result<UserSubscription, SubscriptionError>> Handle(DeleteUserSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Registers a subscription payment.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created payment or an error.</returns>
    Task<Result<SubscriptionPayment, SubscriptionError>> Handle(CreateSubscriptionPaymentCommand command, CancellationToken cancellationToken = default);
}