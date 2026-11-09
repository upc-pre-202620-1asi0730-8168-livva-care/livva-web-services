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
///     Queries the insurers associated with Livva. Requires a valid access token.
/// </summary>
/// <param name="offeringQueryService">The offering query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Insurers")]
public class InsurersController(IOfferingQueryService offeringQueryService, IStringLocalizer<SharedResource> localizer)
    : ControllerBase
{
    /// <summary>Gets all insurers.</summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The insurers.</returns>
    /// <response code="200">The insurers.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InsurerResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetInsurers(CancellationToken cancellationToken)
    {
        var insurers = await offeringQueryService.Handle(new GetAllInsurersQuery(), cancellationToken);
        return Ok(insurers.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets an insurer by its identifier.</summary>
    /// <param name="insurerId">The identifier of the insurer.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The insurer.</returns>
    /// <response code="200">The insurer was found.</response>
    /// <response code="404">The insurer does not exist.</response>
    [HttpGet("{insurerId:long}")]
    [ProducesResponseType(typeof(InsurerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetInsurerById(long insurerId, CancellationToken cancellationToken)
    {
        var insurer = await offeringQueryService.Handle(new GetInsurerByIdQuery(insurerId), cancellationToken);
        return insurer is null
            ? Problem(title: localizer["InsurerNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(insurer));
    }
}