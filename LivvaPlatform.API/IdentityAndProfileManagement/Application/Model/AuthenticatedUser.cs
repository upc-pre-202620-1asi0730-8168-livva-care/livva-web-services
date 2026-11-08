using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;

/// <summary>
///     Result of a successful authentication.
/// </summary>
/// <param name="User">The authenticated user.</param>
/// <param name="Token">The JSON Web Token issued for the user.</param>
public record AuthenticatedUser(User User, string Token);