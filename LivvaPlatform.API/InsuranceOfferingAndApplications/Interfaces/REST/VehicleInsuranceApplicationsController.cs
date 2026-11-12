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
///     Registers and queries the vehicle details of insurance applications. Requires a valid access token.
/// </summary>
/// <param name="commandService">The insurance application command service.</param>
/// <param name="queryService">The insurance application query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Vehicle Insurance Applications")]
public class VehicleInsuranceApplicationsController(
    IInsuranceApplicationCommandService commandService,
    IInsuranceApplicationQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Links an insurance application with a vehicle of the applicant.</summary>
    /// <param name="resource">The vehicle details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created vehicle details.</returns>
    /// <response code="201">The vehicle details were registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The application or the vehicle does not exist.</response>
    /// <response code="409">The application already has details or its product is not a vehicle product.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleInsuranceApplicationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateVehicleInsuranceApplication([FromBody] CreateVehicleInsuranceApplicationResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(ResourceAssembler.ToCommand(resource), cancellationToken);
        return result switch
        {
            Result<VehicleInsuranceApplication, InsuranceApplicationError>.Success s => CreatedAtAction(
                nameof(GetVehicleInsuranceApplicationById), new { id = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<VehicleInsuranceApplication, InsuranceApplicationError>.Failure f =>
                InsuranceApplicationProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets the vehicle details of applications, optionally filtered by application.</summary>
    /// <param name="applicationId">Optional identifier of the insurance application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching vehicle details (empty if none match).</returns>
    /// <response code="200">The matching vehicle details.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleInsuranceApplicationResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVehicleInsuranceApplications([FromQuery] long? applicationId, CancellationToken cancellationToken)
    {
        var details = await queryService.Handle(new GetAllVehicleInsuranceApplicationsQuery(applicationId), cancellationToken);
        return Ok(details.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets vehicle details by identifier.</summary>
    /// <param name="id">The identifier of the vehicle details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicle details.</returns>
    /// <response code="200">The vehicle details were found.</response>
    /// <response code="404">The vehicle details do not exist.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(VehicleInsuranceApplicationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVehicleInsuranceApplicationById(long id, CancellationToken cancellationToken)
    {
        var details = await queryService.Handle(new GetVehicleInsuranceApplicationByIdQuery(id), cancellationToken);
        return details is null
            ? InsuranceApplicationProblemMapper.ToProblem(this, localizer, InsuranceApplicationError.ApplicationNotFound)
            : Ok(ResourceAssembler.ToResource(details));
    }
}