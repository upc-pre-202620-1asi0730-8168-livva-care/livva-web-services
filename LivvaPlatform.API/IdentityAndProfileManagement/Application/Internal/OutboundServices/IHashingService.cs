namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;

/// <summary>
///     Hashes and verifies passwords.
/// </summary>
public interface IHashingService
{
    /// <summary>
    ///     Hashes a plain text password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The password hash.</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verifies a plain text password against a hash.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="passwordHash">The stored hash.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    bool VerifyPassword(string password, string passwordHash);
}