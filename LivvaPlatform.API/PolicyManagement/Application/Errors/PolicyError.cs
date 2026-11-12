namespace LivvaPlatform.API.PolicyManagement.Application.Errors;

/// <summary>
///     Errors that can occur when managing policies and renewals.
/// </summary>
public enum PolicyError
{
    /// <summary>The policyholder user does not exist.</summary>
    UserNotFound,

    /// <summary>The policy does not exist.</summary>
    PolicyNotFound,

    /// <summary>Another policy already uses the policy number.</summary>
    PolicyNumberAlreadyRegistered,

    /// <summary>The policy cannot be renewed in its current status.</summary>
    PolicyNotRenewable,

    /// <summary>The policy already has an open renewal request.</summary>
    RenewalAlreadyRequested,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}