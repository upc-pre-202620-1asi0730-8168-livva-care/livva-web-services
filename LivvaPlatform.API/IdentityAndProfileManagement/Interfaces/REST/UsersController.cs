using System.Net.Mime;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Errors;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Services;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Queries;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.REST;

/// <summary>
///     Queries and updates users. Requires a valid access token.
/// </summary>
/// <param name="userCommandService">The user command service.</param>
/// <param name="userQueryService">The user query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Gets all users, or the users that match an email address.
    /// </summary>
    /// <param name="email">Optional email address used as a filter.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A list of users (empty if none match).</returns>
    /// <response code="200">The matching users.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetUsers([FromQuery] string? email, CancellationToken cancellationToken)
    {
        if (email is not null)
        {
            var user = await userQueryService.Handle(new GetUserByEmailQuery(email), cancellationToken);
            var matches = user is null
                ? []
                : new[] { UserResourceFromEntityAssembler.ToResourceFromEntity(user) };
            return Ok(matches);
        }

        var users = await userQueryService.Handle(new GetAllUsersQuery(), cancellationToken);
        return Ok(users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Gets a user by its identifier.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user.</returns>
    /// <response code="200">The user was found.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    /// <response code="404">The user does not exist.</response>
    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserById(long userId, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId), cancellationToken);
        return user is null
            ? Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    /// <summary>
    ///     Partially updates the personal data of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="resource">The fields to update. Omitted fields keep their current value.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">The user was updated.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    /// <response code="404">The user does not exist.</response>
    [HttpPatch("{userId:long}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUser(long userId, [FromBody] UpdateUserResource resource,
        CancellationToken cancellationToken)
    {
        var command = CommandFromResourceAssembler.ToCommandFromResource(userId, resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<User, UpdateUserError>.Success success =>
                Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<User, UpdateUserError>.Failure { Error: UpdateUserError.UserNotFound } => Problem(
                title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
            _ => Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
        };
    }
}