using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     Fields of a user that can be partially updated. Omitted fields keep their current value.
/// </summary>
/// <param name="FirstName">The new first name.</param>
/// <param name="LastName">The new last name.</param>
/// <param name="Phone">The new phone number.</param>
public record UpdateUserResource(
    [StringLength(100, MinimumLength = 2, ErrorMessage = "InvalidFirstName")] string? FirstName,
    [StringLength(100, MinimumLength = 2, ErrorMessage = "InvalidLastName")] string? LastName,
    [StringLength(30)] string? Phone);