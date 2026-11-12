using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles user profile resources from the UserProfile aggregate.
/// </summary>
public static class UserProfileResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a user profile into a user profile resource.
    /// </summary>
    /// <param name="entity">The user profile.</param>
    /// <returns>The user profile resource.</returns>
    public static UserProfileResource ToResourceFromEntity(UserProfile entity) =>
        new(entity.Id, entity.UserId, entity.DocumentType, entity.DocumentNumber, entity.BirthDate, entity.Address,
            entity.District, entity.Province, entity.Department, entity.Country, entity.CreatedAt, entity.UpdatedAt);
}