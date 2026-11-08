using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;

/// <summary>
///     Credentials used to sign in.
/// </summary>
/// <param name="Email">The email address of the user.</param>
/// <param name="Password">The password of the user.</param>
public record SignInResource(
    [Required(ErrorMessage = "RequiredEmail")] string Email,
    [Required(ErrorMessage = "RequiredPassword")] string Password);