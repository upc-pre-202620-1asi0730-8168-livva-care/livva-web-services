using System.Net.Mime;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Model;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST;

/// <summary>
///     Registers user accounts and authenticates users.
/// </summary>
/// <param name="userCommandService">The user command service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[AllowAnonymous]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Registers a new customer account.
    /// </summary>
    /// <remarks>The email must not be registered. The password is stored as a BCrypt hash.</remarks>
    /// <param name="resource">The sign-up data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created user, without its password hash.</returns>
    /// <response code="201">The account was created.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="409">The email is already registered.</response>
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var command = CommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<User, SignUpError>.Success success => CreatedAtAction(nameof(UsersController.GetUserById),
                "Users", new { userId = success.Value.Id },
                UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<User, SignUpError>.Failure { Error: SignUpError.EmailAlreadyRegistered } => Problem(
                title: localizer["EmailAlreadyRegistered"].Value, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    ///     Authenticates a user and returns an access token.
    /// </summary>
    /// <param name="resource">The credentials.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The authenticated user and its token.</returns>
    /// <response code="200">The credentials are valid.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="401">The credentials are invalid or the account is inactive.</response>
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = CommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<AuthenticatedUser, SignInError>.Success success =>
                Ok(UserResourceFromEntityAssembler.ToResourceFromAuthenticatedUser(success.Value)),
            _ => Problem(title: localizer["InvalidCredentials"].Value, statusCode: StatusCodes.Status401Unauthorized)
        };
    }
}