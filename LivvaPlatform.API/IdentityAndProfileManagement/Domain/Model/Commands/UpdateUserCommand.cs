namespace LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to partially update the personal data of a user.
/// </summary>
/// <remarks>Null values keep the current value.</remarks>
/// <param name="UserId">The identifier of the user.</param>
/// <param name="FirstName">The new first name.</param>
/// <param name="LastName">The new last name.</param>
/// <param name="Phone">The new phone number.</param>
public record UpdateUserCommand(long UserId, string? FirstName, string? LastName, string? Phone);