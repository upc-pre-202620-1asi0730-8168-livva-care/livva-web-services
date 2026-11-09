namespace LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of a policy renewal request.
/// </summary>
public enum RenewalStatus
{
    /// <summary>The renewal was requested and waits for review.</summary>
    PENDING,

    /// <summary>The insurance company is reviewing the renewal.</summary>
    UNDER_REVIEW,

    /// <summary>The renewal was approved.</summary>
    APPROVED,

    /// <summary>The renewal was rejected.</summary>
    REJECTED
}