namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of a life indemnity request.
/// </summary>
public enum IndemnityRequestStatus
{
    /// <summary>The indemnity request was submitted and waits for evaluation.</summary>
    SUBMITTED,

    /// <summary>The insurance company is evaluating the indemnity request.</summary>
    UNDER_REVIEW,

    /// <summary>The indemnity request was approved.</summary>
    APPROVED,

    /// <summary>The indemnity request was rejected.</summary>
    REJECTED,

    /// <summary>The indemnity payment was completed.</summary>
    PAID
}
