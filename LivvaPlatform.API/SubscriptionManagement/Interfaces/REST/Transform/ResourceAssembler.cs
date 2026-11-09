using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;
using LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles resources from the aggregates of the Subscription Management bounded context.
/// </summary>
public static class ResourceAssembler
{
    /// <summary>Converts a plan into a resource.</summary>
    /// <param name="e">The plan.</param>
    /// <returns>The plan resource.</returns>
    public static SubscriptionPlanResource ToResource(SubscriptionPlan e) =>
        new(e.Id, e.Name, e.Description, e.MonthlyPrice, e.AnnualPrice, e.Benefits, e.Active, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a subscription into a resource.</summary>
    /// <param name="e">The subscription.</param>
    /// <returns>The subscription resource.</returns>
    public static UserSubscriptionResource ToResource(UserSubscription e) =>
        new(e.Id, e.UserId, e.PlanId, EnumValueConverter.ToValue(e.BillingCycle), EnumValueConverter.ToValue(e.Status),
            e.StartDate, e.NextBillingDate, e.CancellationRequestedAt, e.CancelledAt, e.CreatedAt, e.UpdatedAt);

    /// <summary>Converts a payment into a resource.</summary>
    /// <param name="e">The payment.</param>
    /// <returns>The payment resource.</returns>
    public static SubscriptionPaymentResource ToResource(SubscriptionPayment e) =>
        new(e.Id, e.UserSubscriptionId, e.UserId, e.PlanId, e.Provider, e.ProviderPaymentId, e.Amount, e.Currency,
            EnumValueConverter.ToValue(e.Status), e.PaidAt, e.CreatedAt, e.UpdatedAt);
}