using LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles user resources from the User aggregate. The password hash is never copied.
/// </summary>
public static class UserResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a user into a user resource.
    /// </summary>
    /// <param name="entity">The user.</param>
    /// <returns>The user resource.</returns>
    public static UserResource ToResourceFromEntity(User entity) =>
        new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Phone, entity.Role.ToString(),
            entity.Active, entity.CreatedAt, entity.UpdatedAt);

    /// <summary>
    ///     Converts an authenticated user into an authenticated user resource.
    /// </summary>
    /// <param name="authenticatedUser">The authenticated user and its token.</param>
    /// <returns>The authenticated user resource.</returns>
    public static AuthenticatedUserResource ToResourceFromAuthenticatedUser(AuthenticatedUser authenticatedUser)
    {
        var user = authenticatedUser.User;
        return new AuthenticatedUserResource(user.Id, user.FirstName, user.LastName, user.Email, user.Phone,
            user.Role.ToString(), user.Active, authenticatedUser.Token);
    }
}