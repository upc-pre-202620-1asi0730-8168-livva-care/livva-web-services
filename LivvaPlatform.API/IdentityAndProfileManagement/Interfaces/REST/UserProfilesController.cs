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
///     Creates, queries and updates user profiles. Requires a valid access token.
/// </summary>
/// <param name="userProfileCommandService">The user profile command service.</param>
/// <param name="userProfileQueryService">The user profile query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("User Profiles")]
public class UserProfilesController(
    IUserProfileCommandService userProfileCommandService,
    IUserProfileQueryService userProfileQueryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Creates the profile of a user.
    /// </summary>
    /// <param name="resource">The profile data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created profile.</returns>
    /// <response code="201">The profile was created.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    /// <response code="404">The user does not exist.</response>
    /// <response code="409">The user already has a profile.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserProfileResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateUserProfile([FromBody] CreateUserProfileResource resource,
        CancellationToken cancellationToken)
    {
        var command = CommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userProfileCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<UserProfile, UserProfileError>.Success success => CreatedAtAction(nameof(GetUserProfileById),
                new { userProfileId = success.Value.Id },
                UserProfileResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<UserProfile, UserProfileError>.Failure failure => ToProblem(failure.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    ///     Gets all user profiles, or the profile of a user.
    /// </summary>
    /// <param name="userId">Optional identifier of the owner user used as a filter.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A list of profiles (empty if none match).</returns>
    /// <response code="200">The matching profiles.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserProfileResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetUserProfiles([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        if (userId is not null)
        {
            var profile = await userProfileQueryService.Handle(new GetUserProfileByUserIdQuery(userId.Value),
                cancellationToken);
            var matches = profile is null
                ? []
                : new[] { UserProfileResourceFromEntityAssembler.ToResourceFromEntity(profile) };
            return Ok(matches);
        }

        var profiles = await userProfileQueryService.Handle(new GetAllUserProfilesQuery(), cancellationToken);
        return Ok(profiles.Select(UserProfileResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Gets a user profile by its identifier.
    /// </summary>
    /// <param name="userProfileId">The identifier of the profile.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The profile.</returns>
    /// <response code="200">The profile was found.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    /// <response code="404">The profile does not exist.</response>
    [HttpGet("{userProfileId:long}")]
    [ProducesResponseType(typeof(UserProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserProfileById(long userProfileId, CancellationToken cancellationToken)
    {
        var profile = await userProfileQueryService.Handle(new GetUserProfileByIdQuery(userProfileId),
            cancellationToken);
        return profile is null
            ? Problem(title: localizer["UserProfileNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(UserProfileResourceFromEntityAssembler.ToResourceFromEntity(profile));
    }

    /// <summary>
    ///     Partially updates a user profile.
    /// </summary>
    /// <param name="userProfileId">The identifier of the profile.</param>
    /// <param name="resource">The fields to update. Omitted fields keep their current value.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated profile.</returns>
    /// <response code="200">The profile was updated.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="401">The access token is missing or invalid.</response>
    /// <response code="404">The profile does not exist.</response>
    [HttpPatch("{userProfileId:long}")]
    [ProducesResponseType(typeof(UserProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUserProfile(long userProfileId,
        [FromBody] UpdateUserProfileResource resource, CancellationToken cancellationToken)
    {
        var command = CommandFromResourceAssembler.ToCommandFromResource(userProfileId, resource);
        var result = await userProfileCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<UserProfile, UserProfileError>.Success success =>
                Ok(UserProfileResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<UserProfile, UserProfileError>.Failure failure => ToProblem(failure.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private ObjectResult ToProblem(UserProfileError error) => error switch
    {
        UserProfileError.UserNotFound => Problem(title: localizer["UserNotFound"].Value,
            statusCode: StatusCodes.Status404NotFound),
        UserProfileError.ProfileNotFound => Problem(title: localizer["UserProfileNotFound"].Value,
            statusCode: StatusCodes.Status404NotFound),
        UserProfileError.ProfileAlreadyExists => Problem(title: localizer["UserProfileAlreadyExists"].Value,
            statusCode: StatusCodes.Status409Conflict),
        _ => Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
    };
}