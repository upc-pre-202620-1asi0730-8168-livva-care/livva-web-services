using LivvaPlatform.API.Shared.Domain.Model;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the SubscriptionPayment aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the AuditableEntityInterceptor of the Shared bounded context.
/// </remarks>
public partial class SubscriptionPayment : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when this payment was created. Maps to column: created_at.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when this payment was last updated. Maps to column: updated_at.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}