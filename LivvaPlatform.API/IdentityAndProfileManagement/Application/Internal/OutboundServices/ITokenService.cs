using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;

/// <summary>
///     Issues access tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Generates an access token for a user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The signed token.</returns>
    string GenerateToken(User user);
}