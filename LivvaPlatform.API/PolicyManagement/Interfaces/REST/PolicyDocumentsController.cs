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
///     Queries the documents of policies. Requires a valid access token.
/// </summary>
/// <param name="queryService">The policy query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Policy Documents")]
public class PolicyDocumentsController(IPolicyQueryService queryService, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Gets policy documents, optionally filtered by policy.</summary>
    /// <param name="policyId">Optional identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching documents (empty if none match).</returns>
    /// <response code="200">The matching documents.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PolicyDocumentResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPolicyDocuments([FromQuery] long? policyId, CancellationToken cancellationToken)
    {
        var items = await queryService.Handle(new GetPolicyDocumentsQuery(policyId), cancellationToken);
        return Ok(items.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a policy document by its identifier.</summary>
    /// <param name="id">The identifier of the document.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The document.</returns>
    /// <response code="200">The document was found.</response>
    /// <response code="404">The document does not exist.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PolicyDocumentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPolicyDocumentById(long id, CancellationToken cancellationToken)
    {
        var item = await queryService.Handle(new GetPolicyDocumentByIdQuery(id), cancellationToken);
        return item is null
            ? Problem(title: localizer["PolicyDocumentNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(item));
    }
}