namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of a vehicle claim. They are exposed in lowercase (for example, <c>under_review</c>).
/// </summary>
public enum VehicleClaimStatus
{
    /// <summary>The claim was reported by the insured.</summary>
    REPORTED,

    /// <summary>The insurance company is reviewing the claim.</summary>
    UNDER_REVIEW,

    /// <summary>The claim was approved.</summary>
    APPROVED,

    /// <summary>The claim was rejected.</summary>
    REJECTED,

    /// <summary>The claim was resolved.</summary>
    RESOLVED,

    /// <summary>The claim was cancelled by the insured.</summary>
    CANCELLED
}
