namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;

/// <summary>
///     Errors that can occur when managing vehicles.
/// </summary>
public enum VehicleError
{
    /// <summary>The owner user does not exist.</summary>
    UserNotFound,

    /// <summary>The vehicle does not exist.</summary>
    VehicleNotFound,

    /// <summary>Another vehicle already uses the license plate.</summary>
    LicensePlateAlreadyRegistered,

    /// <summary>The vehicle is used by an insurance application and cannot be deleted.</summary>
    VehicleInUse,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}