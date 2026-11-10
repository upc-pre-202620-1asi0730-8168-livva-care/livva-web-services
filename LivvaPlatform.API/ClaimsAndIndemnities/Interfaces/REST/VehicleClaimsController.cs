
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
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

    [HttpGet("{claimId:long}")]
    [ProducesResponseType(typeof(VehicleClaimResource), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById(
        long claimId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimByIdQuery(claimId),
            cancellationToken);

        return result switch
        {
            Result<VehicleClaim, VehicleClaimError>.Success success =>
                Ok(VehicleClaimAssembler.ToResource(success.Value)),

            Result<VehicleClaim, VehicleClaimError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("user/{userId:long}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleClaimResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByUser(
        long userId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimsByUserIdQuery(userId),
            cancellationToken);

        return result switch
        {
            Result<IEnumerable<VehicleClaim>, VehicleClaimError>.Success success =>
                Ok(success.Value.Select(VehicleClaimAssembler.ToResource)),

            Result<IEnumerable<VehicleClaim>, VehicleClaimError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("policy/{policyId:long}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleClaimResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByPolicy(
        long policyId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetVehicleClaimsByPolicyIdQuery(policyId),
            cancellationToken);

        return result switch
        {
            Result<IEnumerable<VehicleClaim>, VehicleClaimError>.Success success =>
                Ok(success.Value.Select(VehicleClaimAssembler.ToResource)),

            Result<IEnumerable<VehicleClaim>, VehicleClaimError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

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

    private ActionResult MapError(VehicleClaimError error)
    {
        var statusCode = error switch
        {
            VehicleClaimError.UserNotFound => StatusCodes.Status404NotFound,
            VehicleClaimError.PolicyNotFound => StatusCodes.Status404NotFound,
            VehicleClaimError.VehicleClaimNotFound => StatusCodes.Status404NotFound,
            VehicleClaimError.InvalidStatusChange => StatusCodes.Status409Conflict,
            VehicleClaimError.InvalidData => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            title: error.ToString(),
            statusCode: statusCode);
    }
}
