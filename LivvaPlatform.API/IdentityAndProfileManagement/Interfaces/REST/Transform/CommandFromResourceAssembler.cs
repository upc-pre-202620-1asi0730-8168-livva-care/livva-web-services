using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles commands from the request resources of the Identity and Profile Management bounded context.
/// </summary>
public static class CommandFromResourceAssembler
{
    /// <summary>Converts a sign-up resource into a sign-up command.</summary>
    /// <param name="resource">The sign-up resource.</param>
    /// <returns>The sign-up command.</returns>
    public static SignUpCommand ToCommandFromResource(SignUpResource resource) =>
        new(resource.FirstName, resource.LastName, resource.Email, resource.Password, resource.Phone);

    /// <summary>Converts a sign-in resource into a sign-in command.</summary>
    /// <param name="resource">The sign-in resource.</param>
    /// <returns>The sign-in command.</returns>
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Email, resource.Password);

    /// <summary>Converts an update user resource into an update user command.</summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="resource">The update user resource.</param>
    /// <returns>The update user command.</returns>
    public static UpdateUserCommand ToCommandFromResource(long userId, UpdateUserResource resource) =>
        new(userId, resource.FirstName, resource.LastName, resource.Phone);

    /// <summary>Converts a create user profile resource into a create user profile command.</summary>
    /// <param name="resource">The create user profile resource.</param>
    /// <returns>The create user profile command.</returns>
    public static CreateUserProfileCommand ToCommandFromResource(CreateUserProfileResource resource) =>
        new(resource.UserId, resource.DocumentType, resource.DocumentNumber, resource.BirthDate, resource.Address,
            resource.District, resource.Province, resource.Department, resource.Country);

    /// <summary>Converts an update user profile resource into an update user profile command.</summary>
    /// <param name="userProfileId">The identifier of the profile.</param>
    /// <param name="resource">The update user profile resource.</param>
    /// <returns>The update user profile command.</returns>
    public static UpdateUserProfileCommand ToCommandFromResource(long userProfileId,
        UpdateUserProfileResource resource) =>
        new(userProfileId, resource.DocumentType, resource.DocumentNumber, resource.BirthDate, resource.Address,
            resource.District, resource.Province, resource.Department, resource.Country);
}