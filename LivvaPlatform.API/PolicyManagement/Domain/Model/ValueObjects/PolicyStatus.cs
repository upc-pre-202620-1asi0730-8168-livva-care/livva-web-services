namespace LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of an insurance policy.
/// </summary>
public enum PolicyStatus
{
    /// <summary>The policy was issued but is not active yet.</summary>
    PENDING,

    /// <summary>The policy is in force.</summary>
    ACTIVE,

    /// <summary>The policy reached its expiration date.</summary>
    EXPIRED,

    /// <summary>The policy was cancelled.</summary>
    CANCELLED,

    /// <summary>The policy has a renewal request in progress.</summary>
    PENDING_RENEWAL
}