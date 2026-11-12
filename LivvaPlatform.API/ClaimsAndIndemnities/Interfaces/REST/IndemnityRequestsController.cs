
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
[Route("api/v1/indemnity-requests")]
[Produces("application/json")]
[Tags("Indemnity Requests")]
public class IndemnityRequestsController(
    IIndemnityRequestCommandService commandService,
    IIndemnityRequestQueryService queryService) : ControllerBase
{
    // POST: api/v1/indemnity-requests
    [HttpPost]
    [ProducesResponseType(typeof(IndemnityRequestResource), StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(
        [FromBody] CreateIndemnityRequestResource resource,
        CancellationToken cancellationToken)
    {
        var command = IndemnityRequestAssembler.ToCommand(resource);

        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<IndemnityRequest, IndemnityRequestError>.Success success =>
                CreatedAtAction(
                    nameof(GetById),
                    new { requestId = success.Value.Id },
                    IndemnityRequestAssembler.ToResource(success.Value)),

            Result<IndemnityRequest, IndemnityRequestError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    // GET: api/v1/indemnity-requests/{requestId}
    [HttpGet("{requestId:long}")]
    [ProducesResponseType(typeof(IndemnityRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(
        long requestId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(
            new GetIndemnityRequestByIdQuery(requestId),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(IndemnityRequestAssembler.ToResource(result));
    }

    /// <summary>Gets indemnity requests filtered by <c>policyId</c> or <c>beneficiaryId</c>.</summary>
    /// <param name="policyId">Optional policy identifier.</param>
    /// <param name="beneficiaryId">Optional beneficiary identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="200">The indemnity requests (empty if none match or no filter is given).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IndemnityRequestResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetIndemnityRequests([FromQuery] long? policyId, [FromQuery] long? beneficiaryId,
        CancellationToken cancellationToken)
    {
        IEnumerable<IndemnityRequest> result = policyId is not null
            ? await queryService.Handle(new GetIndemnityRequestsByPolicyIdQuery(policyId.Value), cancellationToken)
            : beneficiaryId is not null
                ? await queryService.Handle(new GetIndemnityRequestsByBeneficiaryIdQuery(beneficiaryId.Value),
                    cancellationToken)
                : [];

        return Ok(result.Select(IndemnityRequestAssembler.ToResource));
    }

    // PATCH: api/v1/indemnity-requests/{requestId}
    [HttpPatch("{requestId:long}")]
    [ProducesResponseType(typeof(IndemnityRequestResource), StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangeStatus(
        long requestId,
        [FromBody] ChangeIndemnityRequestStatusResource resource,
        CancellationToken cancellationToken)
    {
        var command = IndemnityRequestAssembler.ToCommand(requestId, resource);

        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<IndemnityRequest, IndemnityRequestError>.Success success =>
                Ok(IndemnityRequestAssembler.ToResource(success.Value)),

            Result<IndemnityRequest, IndemnityRequestError>.Failure failure =>
                MapError(failure.Error),

            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    // Maps domain errors to HTTP responses.
    private ActionResult MapError(IndemnityRequestError error)
    {
        var statusCode = error switch
        {
            IndemnityRequestError.PolicyNotFound =>
                StatusCodes.Status404NotFound,

            IndemnityRequestError.BeneficiaryNotFound =>
                StatusCodes.Status404NotFound,

            IndemnityRequestError.IndemnityRequestNotFound =>
                StatusCodes.Status404NotFound,

            IndemnityRequestError.InvalidStatusChange =>
                StatusCodes.Status409Conflict,

            IndemnityRequestError.InvalidData =>
                StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            title: error.ToString(),
            statusCode: statusCode);
    }
}

