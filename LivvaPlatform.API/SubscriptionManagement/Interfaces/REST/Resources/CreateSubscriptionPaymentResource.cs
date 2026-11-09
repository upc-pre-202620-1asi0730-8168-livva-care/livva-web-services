using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>Data of a payment processed by the payment provider for a subscription.</summary>
public record CreateSubscriptionPaymentResource(
    [Range(1, long.MaxValue, ErrorMessage = "RequiredUserSubscriptionId")] long UserSubscriptionId,
    long UserId,
    long PlanId,
    [Required(ErrorMessage = "RequiredProvider")] [StringLength(50)] string Provider,
    [StringLength(100)] string? ProviderPaymentId,
    [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "InvalidAmount")] decimal Amount,
    [Required] [StringLength(3, MinimumLength = 3)] string Currency,
    [Required(ErrorMessage = "InvalidPaymentStatus")] string Status,
    DateTimeOffset? PaidAt);
