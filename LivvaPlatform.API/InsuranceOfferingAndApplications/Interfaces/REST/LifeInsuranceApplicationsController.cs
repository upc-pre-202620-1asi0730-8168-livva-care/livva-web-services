using System.Net.Mime;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST;
/// <summary>
///     Registers and queries the life details of insurance applications. Requires a valid access token.
/// </summary>
/// <param name="commandService">The insurance application command service.</param>
/// <param name="queryService">The insurance application query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Life Insurance Applications")]
public class LifeInsuranceApplicationsController(
    IInsuranceApplicationCommandService commandService,
    IInsuranceApplicationQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Marks an insurance application as a life insurance application.</summary>
    /// <param name="resource">The life details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created life details.</returns>
    /// <response code="201">The life details were registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The application does not exist.</response>
    /// <response code="409">The application already has details or its product is not a life product.</response>
    [HttpPost]
    [ProducesResponseType(typeof(LifeInsuranceApplicationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateLifeInsuranceApplication([FromBody] CreateLifeInsuranceApplicationResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(ResourceAssembler.ToCommand(resource), cancellationToken);
        return result switch
        {
            Result<LifeInsuranceApplication, InsuranceApplicationError>.Success s => CreatedAtAction(
                nameof(GetLifeInsuranceApplicationById), new { id = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<LifeInsuranceApplication, InsuranceApplicationError>.Failure f =>
                InsuranceApplicationProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets the life details of applications, optionally filtered by application.</summary>
    /// <param name="applicationId">Optional identifier of the insurance application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching life details (empty if none match).</returns>
    /// <response code="200">The matching life details.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LifeInsuranceApplicationResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetLifeInsuranceApplications([FromQuery] long? applicationId, CancellationToken cancellationToken)
    {
        var details = await queryService.Handle(new GetAllLifeInsuranceApplicationsQuery(applicationId), cancellationToken);
        return Ok(details.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets life details by identifier.</summary>
    /// <param name="id">The identifier of the life details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The life details.</returns>
    /// <response code="200">The life details were found.</response>
    /// <response code="404">The life details do not exist.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(LifeInsuranceApplicationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLifeInsuranceApplicationById(long id, CancellationToken cancellationToken)
    {
        var details = await queryService.Handle(new GetLifeInsuranceApplicationByIdQuery(id), cancellationToken);
        return details is null
            ? InsuranceApplicationProblemMapper.ToProblem(this, localizer, InsuranceApplicationError.ApplicationNotFound)
            : Ok(ResourceAssembler.ToResource(details));
    }
}