
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

[ApiController]
[Authorize]
[Route("api/v1/vehicle-claims")]
[Produces("application/json")]
[Tags("Vehicle Claims")]
public class VehicleClaimsController(
    IVehicleClaimCommandService commandService,
    IVehicleClaimQueryService queryService) : ControllerBase
{
    // POST: api/v1/vehicle-claims
    [HttpPost]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(
        [FromBody] CreateVehicleClaimResource resource,
        CancellationToken cancellationToken)
    {
        var command = VehicleClaimAssembler.ToCommand(resource);

        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<VehicleClaim, VehicleClaimError>.Success success =>
                CreatedAtAction(
                    nameof(GetById),
                    new { claimId = success.Value.Id },
                    VehicleClaimAssembler.ToResource(success.Value)),

            Result<VehicleClaim, VehicleClaimError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    // GET: api/v1/vehicle-claims/{claimId}
    [HttpGet("{claimId:long}")]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(
        long claimId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimByIdQuery(claimId),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(VehicleClaimAssembler.ToResource(result));
    }

    // GET: api/v1/vehicle-claims/user/{userId}
    [HttpGet("user/{userId:long}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleClaimResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByUser(
        long userId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimsByUserIdQuery(userId),
            cancellationToken);

        return Ok(result.Select(VehicleClaimAssembler.ToResource));
    }

    // GET: api/v1/vehicle-claims/policy/{policyId}
    [HttpGet("policy/{policyId:long}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleClaimResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByPolicy(
        long policyId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimsByPolicyIdQuery(policyId),
            cancellationToken);

        return Ok(result.Select(VehicleClaimAssembler.ToResource));
    }

    // PATCH: api/v1/vehicle-claims/{claimId}/status
    [HttpPatch("{claimId:long}/status")]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangeStatus(
        long claimId,
        [FromBody] ChangeVehicleClaimStatusResource resource,
        CancellationToken cancellationToken)
    {
        var command = VehicleClaimAssembler.ToCommand(claimId, resource);

        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<VehicleClaim, VehicleClaimError>.Success success =>
                Ok(VehicleClaimAssembler.ToResource(success.Value)),

            Result<VehicleClaim, VehicleClaimError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    // Maps domain errors to HTTP responses.
    private ActionResult MapError(VehicleClaimError error)
    {
        var statusCode = error switch
        {
            VehicleClaimError.UserNotFound =>
                StatusCodes.Status404NotFound,

            VehicleClaimError.PolicyNotFound =>
                StatusCodes.Status404NotFound,

            VehicleClaimError.VehicleClaimNotFound =>
                StatusCodes.Status404NotFound,

            VehicleClaimError.InvalidStatusChange =>
                StatusCodes.Status409Conflict,

            VehicleClaimError.InvalidData =>
                StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            title: error.ToString(),
            statusCode: statusCode);
    }
}
