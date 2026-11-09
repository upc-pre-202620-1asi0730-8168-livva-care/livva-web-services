using System.Net.Mime;
using System.Security.Claims;
using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Application.Services;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;
using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;
using LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;
using LivvaPlatform.API.PolicyManagement.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST;
/// <summary>
///     Requests and queries policy renewals. Requires a valid access token.
/// </summary>
/// <param name="commandService">The policy command service.</param>
/// <param name="queryService">The policy query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Renewals")]
public class RenewalsController(
    IPolicyCommandService commandService,
    IPolicyQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Requests the renewal of a policy. New renewals start as <c>pending</c>.</summary>
    /// <param name="resource">The renewal data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created renewal.</returns>
    /// <response code="201">The renewal was requested.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The policy does not exist.</response>
    /// <response code="409">The policy cannot be renewed or already has an open renewal.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RenewalResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateRenewal([FromBody] CreateRenewalResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new CreateRenewalCommand(resource.PolicyId), cancellationToken);
        return result switch
        {
            Result<Renewal, PolicyError>.Success s => CreatedAtAction(nameof(GetRenewalById), new { renewalId = s.Value.Id },
                ResourceAssembler.ToResource(s.Value)),
            Result<Renewal, PolicyError>.Failure f => PolicyProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets renewals, optionally filtered by policy.</summary>
    /// <param name="policyId">Optional identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching renewals (empty if none match).</returns>
    /// <response code="200">The matching renewals.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RenewalResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRenewals([FromQuery] long? policyId, CancellationToken cancellationToken)
    {
        var renewals = await queryService.Handle(new GetRenewalsQuery(policyId), cancellationToken);
        return Ok(renewals.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a renewal by its identifier.</summary>
    /// <param name="renewalId">The identifier of the renewal.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The renewal.</returns>
    /// <response code="200">The renewal was found.</response>
    /// <response code="404">The renewal does not exist.</response>
    [HttpGet("{renewalId:long}")]
    [ProducesResponseType(typeof(RenewalResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRenewalById(long renewalId, CancellationToken cancellationToken)
    {
        var renewal = await queryService.Handle(new GetRenewalByIdQuery(renewalId), cancellationToken);
        return renewal is null
            ? Problem(title: localizer["RenewalNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(renewal));
    }
}