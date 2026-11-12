namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;

/// <summary>
///     Statuses of an insurance application.
/// </summary>
public enum RequestStatus
{
    /// <summary>The application was submitted and waits for evaluation.</summary>
    PENDING,

    /// <summary>The insurance company is evaluating the application.</summary>
    UNDER_REVIEW,

    /// <summary>The application was approved.</summary>
    APPROVED,

    /// <summary>The application was rejected.</summary>
    REJECTED,

    /// <summary>The application was cancelled.</summary>
    CANCELLED
}