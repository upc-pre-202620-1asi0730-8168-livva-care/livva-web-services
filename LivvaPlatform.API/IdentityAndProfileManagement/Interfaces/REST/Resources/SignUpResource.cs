using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     Data required to register a user account.
/// </summary>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
/// <param name="Email">The email address used to sign in.</param>
/// <param name="Password">The password, with at least 6 characters.</param>
/// <param name="Phone">The phone number of the user.</param>
public record SignUpResource(
    [Required(ErrorMessage = "RequiredFirstName")] [StringLength(100, MinimumLength = 2, ErrorMessage = "InvalidFirstName")]
    string FirstName,
    [Required(ErrorMessage = "RequiredLastName")] [StringLength(100, MinimumLength = 2, ErrorMessage = "InvalidLastName")]
    string LastName,
    [Required(ErrorMessage = "RequiredEmail")] [EmailAddress(ErrorMessage = "InvalidEmail")] [StringLength(255)]
    string Email,
    [Required(ErrorMessage = "RequiredPassword")] [MinLength(6, ErrorMessage = "InvalidPassword")]
    string Password,
    [StringLength(30)] string? Phone);