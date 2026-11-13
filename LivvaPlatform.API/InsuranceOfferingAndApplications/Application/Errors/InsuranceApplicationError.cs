namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;

/// <summary>
///     Errors that can occur when managing insurance applications.
/// </summary>
public enum InsuranceApplicationError
{
    /// <summary>The applicant user does not exist.</summary>
    UserNotFound,

    /// <summary>The insurance product does not exist or is inactive.</summary>
    ProductNotAvailable,

    /// <summary>The insurance application does not exist.</summary>
    ApplicationNotFound,

    /// <summary>The vehicle does not exist or does not belong to the applicant.</summary>
    VehicleNotFound,

    /// <summary>The product type does not match the kind of application.</summary>
    ProductTypeMismatch,

    /// <summary>The application already has vehicle or life details.</summary>
    ApplicationAlreadyLinked,

    /// <summary>The application cannot change to the requested status.</summary>
    InvalidStatusChange,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}