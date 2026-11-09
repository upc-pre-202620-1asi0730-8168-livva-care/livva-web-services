using LivvaPlatform.API.Shared.Domain.Model;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the Vehicle aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the AuditableEntityInterceptor of the Shared bounded context.
/// </remarks>
public partial class Vehicle : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when this vehicle was created. Maps to column: created_at.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when this vehicle was last updated. Maps to column: updated_at.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}