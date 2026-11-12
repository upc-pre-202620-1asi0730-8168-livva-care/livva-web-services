using System.Security.Claims;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Transform;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST;

/// <summary>
///     Vehicle claims reported by insured users.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/vehicle-claims")]
[Produces("application/json")]
[Tags("Vehicle Claims")]
public class VehicleClaimsController(
    IVehicleClaimCommandService commandService,
    IVehicleClaimQueryService queryService) : ControllerBase
{
    /// <summary>Gets vehicle claims.</summary>
    /// <remarks>
    ///     Filters by <c>policyId</c> or <c>userId</c>. Without filters, returns the claims of the authenticated user.
    /// </remarks>
    /// <param name="policyId">Optional policy identifier.</param>
    /// <param name="userId">Optional user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="200">The vehicle claims (empty if none match).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleClaimResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVehicleClaims([FromQuery] long? policyId, [FromQuery] long? userId,
        CancellationToken cancellationToken)
    {
        var claims = policyId is not null
            ? await queryService.Handle(new GetVehicleClaimsByPolicyIdQuery(policyId.Value), cancellationToken)
            : await queryService.Handle(new GetVehicleClaimsByUserIdQuery(userId ?? CurrentUserId()),
                cancellationToken);

        return Ok(claims.Select(VehicleClaimAssembler.ToResource));
    }

    /// <summary>Gets a vehicle claim by its identifier.</summary>
    /// <param name="claimId">The identifier of the claim.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="200">The vehicle claim.</response>
    /// <response code="404">The vehicle claim does not exist.</response>
    [HttpGet("{claimId:long}")]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(long claimId, CancellationToken cancellationToken)
    {
        var claim = await queryService.Handle(new GetVehicleClaimByIdQuery(claimId), cancellationToken);
        return claim is null
            ? MapError(VehicleClaimError.VehicleClaimNotFound)
            : Ok(VehicleClaimAssembler.ToResource(claim));
    }

    /// <summary>Reports a vehicle claim.</summary>
    /// <remarks>The claim starts as <c>reported</c> and its first status history entry is recorded.</remarks>
    /// <param name="resource">The claim data. Without <c>userId</c>, the authenticated user is used.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="201">The claim was reported.</response>
    /// <response code="400">The data is not valid.</response>
    /// <response code="404">The user or the policy does not exist.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Create([FromBody] CreateVehicleClaimResource resource,
        CancellationToken cancellationToken)
    {
        var command = VehicleClaimAssembler.ToCommand(resource, resource.UserId ?? CurrentUserId());
        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<VehicleClaim, VehicleClaimError>.Success success =>
                CreatedAtAction(nameof(GetById), new { claimId = success.Value.Id },
                    VehicleClaimAssembler.ToResource(success.Value)),
            Result<VehicleClaim, VehicleClaimError>.Failure failure => MapError(failure.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Changes the status of a vehicle claim.</summary>
    /// <remarks>For example, <c>{"status": "cancelled"}</c>. Each change is recorded in the status history.</remarks>
    /// <param name="claimId">The identifier of the claim.</param>
    /// <param name="resource">The new status and an optional comment.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="200">The updated claim.</response>
    /// <response code="400">The status is not valid.</response>
    /// <response code="404">The claim does not exist.</response>
    /// <response code="409">The claim cannot change to that status.</response>
    [HttpPatch("{claimId:long}")]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> ChangeStatus(long claimId, [FromBody] ChangeVehicleClaimStatusResource resource,
        CancellationToken cancellationToken)
    {
        var command = VehicleClaimAssembler.ToCommand(claimId, resource);
        if (command is null) return MapError(VehicleClaimError.InvalidData);

        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<VehicleClaim, VehicleClaimError>.Success success =>
                Ok(VehicleClaimAssembler.ToResource(success.Value)),
            Result<VehicleClaim, VehicleClaimError>.Failure failure => MapError(failure.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private long CurrentUserId() =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private ActionResult MapError(VehicleClaimError error)
    {
        var statusCode = error switch
        {
            VehicleClaimError.UserNotFound or VehicleClaimError.PolicyNotFound
                or VehicleClaimError.VehicleClaimNotFound => StatusCodes.Status404NotFound,
            VehicleClaimError.InvalidStatusChange => StatusCodes.Status409Conflict,
            VehicleClaimError.InvalidData => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(title: error.ToString(), statusCode: statusCode);
    }
}
