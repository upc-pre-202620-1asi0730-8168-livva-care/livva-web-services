using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
///     BCrypt implementation of the hashing service.
/// </summary>
public class HashingService : IHashingService
{
    /// <inheritdoc />
    public string HashPassword(string password) => BCryptNet.HashPassword(password);

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCryptNet.Verify(password, passwordHash);
        }
        catch (global::BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}