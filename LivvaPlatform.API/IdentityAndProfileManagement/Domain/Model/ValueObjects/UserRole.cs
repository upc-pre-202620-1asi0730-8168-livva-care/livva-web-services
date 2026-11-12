namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.ValueObjects;

/// <summary>
///     Roles that a user can have in the Livva platform.
/// </summary>
public enum UserRole
{
    /// <summary>
    ///     A customer who requests and manages insurance through Livva.
    /// </summary>
    CUSTOMER,

    /// <summary>
    ///     A representative of an associated insurance company.
    /// </summary>
    INSURANCE_REPRESENTATIVE,

    /// <summary>
    ///     A Livva platform administrator.
    /// </summary>
    ADMIN
}