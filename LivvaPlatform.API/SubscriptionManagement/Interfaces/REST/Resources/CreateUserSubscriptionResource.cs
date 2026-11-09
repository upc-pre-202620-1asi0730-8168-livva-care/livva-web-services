using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>Data required to subscribe a user to a plan. The subscription starts as <c>active</c> and the dates are calculated by the API.</summary>
public record CreateUserSubscriptionResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "RequiredPlanId")] long PlanId,
    [Required(ErrorMessage = "InvalidBillingCycle")] string BillingCycle);
