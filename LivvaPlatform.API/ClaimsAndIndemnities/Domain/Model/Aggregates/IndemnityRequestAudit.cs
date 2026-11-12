using LivvaPlatform.API.Shared.Domain.Model;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the IndemnityRequest aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are managed by the persistence layer
///     through the shared auditing mechanism.
/// </remarks>
public partial class IndemnityRequest : IAuditableEntity
{
    /// <summary>
    ///     Gets the timestamp when the indemnity request was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets the timestamp when the indemnity request was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
