namespace LivvaPlatform.API.SubscriptionManagement.Application.Errors;

/// <summary>
///     Errors that can occur when managing subscriptions and payments.
/// </summary>
public enum SubscriptionError
{
    /// <summary>The user does not exist.</summary>
    UserNotFound,

    /// <summary>The plan does not exist or is inactive.</summary>
    PlanNotAvailable,

    /// <summary>The user already has an active subscription.</summary>
    ActiveSubscriptionExists,

    /// <summary>The subscription does not exist.</summary>
    SubscriptionNotFound,

    /// <summary>The subscription is not active.</summary>
    SubscriptionNotActive,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}