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
///     Registers, queries and updates beneficiaries of life insurance policies. Requires a valid access token.
/// </summary>
/// <param name="commandService">The beneficiary command service.</param>
/// <param name="queryService">The beneficiary query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Beneficiaries")]
public class BeneficiariesController(
    IBeneficiaryCommandService commandService,
    IBeneficiaryQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Registers a beneficiary of a life insurance policy.</summary>
    /// <param name="resource">The beneficiary data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created beneficiary.</returns>
    /// <response code="201">The beneficiary was registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BeneficiaryResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateBeneficiary([FromBody] CreateBeneficiaryResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(ResourceAssembler.ToCommand(resource), cancellationToken);
        return result switch
        {
            Result<Beneficiary, BeneficiaryError>.Success s => CreatedAtAction(nameof(GetBeneficiaryById),
                new { beneficiaryId = s.Value.Id }, ResourceAssembler.ToResource(s.Value)),
            Result<Beneficiary, BeneficiaryError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets beneficiaries, optionally filtered by policy.</summary>
    /// <param name="policyId">Optional identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching beneficiaries (empty if none match).</returns>
    /// <response code="200">The matching beneficiaries.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BeneficiaryResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBeneficiaries([FromQuery] long? policyId, CancellationToken cancellationToken)
    {
        var beneficiaries = await queryService.Handle(new GetAllBeneficiariesQuery(policyId), cancellationToken);
        return Ok(beneficiaries.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a beneficiary by its identifier.</summary>
    /// <param name="beneficiaryId">The identifier of the beneficiary.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The beneficiary.</returns>
    /// <response code="200">The beneficiary was found.</response>
    /// <response code="404">The beneficiary does not exist.</response>
    [HttpGet("{beneficiaryId:long}")]
    [ProducesResponseType(typeof(BeneficiaryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBeneficiaryById(long beneficiaryId, CancellationToken cancellationToken)
    {
        var beneficiary = await queryService.Handle(new GetBeneficiaryByIdQuery(beneficiaryId), cancellationToken);
        return beneficiary is null ? ToProblem(BeneficiaryError.BeneficiaryNotFound) : Ok(ResourceAssembler.ToResource(beneficiary));
    }

    /// <summary>Partially updates a beneficiary.</summary>
    /// <param name="beneficiaryId">The identifier of the beneficiary.</param>
    /// <param name="resource">The fields to update. Omitted fields keep their current value.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated beneficiary.</returns>
    /// <response code="200">The beneficiary was updated.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The beneficiary does not exist.</response>
    [HttpPatch("{beneficiaryId:long}")]
    [ProducesResponseType(typeof(BeneficiaryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBeneficiary(long beneficiaryId, [FromBody] UpdateBeneficiaryResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(ResourceAssembler.ToCommand(beneficiaryId, resource), cancellationToken);
        return result switch
        {
            Result<Beneficiary, BeneficiaryError>.Success s => Ok(ResourceAssembler.ToResource(s.Value)),
            Result<Beneficiary, BeneficiaryError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private ObjectResult ToProblem(BeneficiaryError error) => error switch
    {
        BeneficiaryError.BeneficiaryNotFound => Problem(title: localizer["BeneficiaryNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        _ => Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
    };
}