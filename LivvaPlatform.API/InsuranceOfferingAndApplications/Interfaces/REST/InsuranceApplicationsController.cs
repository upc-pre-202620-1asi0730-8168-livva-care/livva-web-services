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
///     Submits, queries and deletes insurance applications. Requires a valid access token.
/// </summary>
/// <param name="commandService">The insurance application command service.</param>
/// <param name="queryService">The insurance application query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Insurance Applications")]
public class InsuranceApplicationsController(
    IInsuranceApplicationCommandService commandService,
    IInsuranceApplicationQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Submits an insurance application. New applications start as <c>pending</c>.</summary>
    /// <param name="resource">The application data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created application.</returns>
    /// <response code="201">The application was submitted.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The user does not exist or the product is not available.</response>
    [HttpPost]
    [ProducesResponseType(typeof(InsuranceApplicationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateInsuranceApplication([FromBody] CreateInsuranceApplicationResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(ResourceAssembler.ToCommand(resource), cancellationToken);
        return result switch
        {
            Result<InsuranceApplication, InsuranceApplicationError>.Success s => CreatedAtAction(
                nameof(GetInsuranceApplicationById), new { applicationId = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<InsuranceApplication, InsuranceApplicationError>.Failure f =>
                InsuranceApplicationProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets insurance applications, optionally filtered by applicant.</summary>
    /// <param name="userId">Optional identifier of the applicant user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching applications (empty if none match).</returns>
    /// <response code="200">The matching applications.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InsuranceApplicationResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetInsuranceApplications([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        var applications = await queryService.Handle(new GetAllInsuranceApplicationsQuery(userId), cancellationToken);
        return Ok(applications.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets an insurance application by its identifier.</summary>
    /// <param name="applicationId">The identifier of the application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The application.</returns>
    /// <response code="200">The application was found.</response>
    /// <response code="404">The application does not exist.</response>
    [HttpGet("{applicationId:long}")]
    [ProducesResponseType(typeof(InsuranceApplicationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetInsuranceApplicationById(long applicationId, CancellationToken cancellationToken)
    {
        var application = await queryService.Handle(new GetInsuranceApplicationByIdQuery(applicationId), cancellationToken);
        return application is null
            ? InsuranceApplicationProblemMapper.ToProblem(this, localizer, InsuranceApplicationError.ApplicationNotFound)
            : Ok(ResourceAssembler.ToResource(application));
    }

    /// <summary>Deletes an insurance application together with its vehicle or life details.</summary>
    /// <param name="applicationId">The identifier of the application.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The application was deleted.</response>
    /// <response code="404">The application does not exist.</response>
    [HttpDelete("{applicationId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteInsuranceApplication(long applicationId, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new DeleteInsuranceApplicationCommand(applicationId), cancellationToken);
        return result switch
        {
            Result<InsuranceApplication, InsuranceApplicationError>.Success => NoContent(),
            Result<InsuranceApplication, InsuranceApplicationError>.Failure f =>
                InsuranceApplicationProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}