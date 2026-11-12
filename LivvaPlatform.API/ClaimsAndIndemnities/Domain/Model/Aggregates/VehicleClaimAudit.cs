using LivvaPlatform.API.Shared.Domain.Model;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the VehicleClaim aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the
///     AuditableEntityInterceptor of the Shared bounded context.
/// </remarks>
public partial class VehicleClaim : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when this vehicle claim was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when this vehicle claim was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}