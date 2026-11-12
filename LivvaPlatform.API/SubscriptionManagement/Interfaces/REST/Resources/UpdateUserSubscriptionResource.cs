using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.SubscriptionManagement.Interfaces.REST.Resources;

/// <summary>Partial update of a user subscription. Only the status <c>cancelled</c> is supported.</summary>
public record UpdateUserSubscriptionResource([Required(ErrorMessage = "InvalidSubscriptionStatus")] string Status);
