namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;

/// <summary>
///     Errors that can occur when managing vehicle claims.
/// </summary>
public enum VehicleClaimError
{
    /// <summary>The user associated with the claim does not exist.</summary>
    UserNotFound,

    /// <summary>The related policy does not exist.</summary>
    PolicyNotFound,

    /// <summary>The vehicle claim does not exist.</summary>
    VehicleClaimNotFound,

    /// <summary>The vehicle claim cannot be updated to the requested status.</summary>
    InvalidStatusChange,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}