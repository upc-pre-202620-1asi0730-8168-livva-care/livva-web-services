using LivvaPlatform.API.Shared.Domain.Model;

namespace LivvaPlatform.API.SubscriptionManagement.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the UserSubscription aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the AuditableEntityInterceptor of the Shared bounded context.
/// </remarks>
public partial class UserSubscription : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when this subscription was created. Maps to column: created_at.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when this subscription was last updated. Maps to column: updated_at.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}