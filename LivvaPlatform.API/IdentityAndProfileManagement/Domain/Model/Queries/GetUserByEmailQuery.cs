namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a user by its email address.
/// </summary>
public record GetUserByEmailQuery(string Email);
