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
///     Queries and registers insurance policies. Requires a valid access token.
/// </summary>
/// <param name="commandService">The policy command service.</param>
/// <param name="queryService">The policy query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Policies")]
public class PoliciesController(
    IPolicyCommandService commandService,
    IPolicyQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Gets the policies of a user. Without <c>userId</c>, returns the policies of the authenticated user.</summary>
    /// <param name="userId">Optional identifier of the policyholder.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The policies (empty if none match).</returns>
    /// <response code="200">The policies.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PolicyResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPolicies([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        var ownerId = userId ?? (long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0);
        var policies = await queryService.Handle(new GetPoliciesByUserIdQuery(ownerId), cancellationToken);
        return Ok(policies.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a policy by its identifier.</summary>
    /// <param name="policyId">The identifier of the policy.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The policy.</returns>
    /// <response code="200">The policy was found.</response>
    /// <response code="404">The policy does not exist.</response>
    [HttpGet("{policyId:long}")]
    [ProducesResponseType(typeof(PolicyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPolicyById(long policyId, CancellationToken cancellationToken)
    {
        var policy = await queryService.Handle(new GetPolicyByIdQuery(policyId), cancellationToken);
        return policy is null
            ? PolicyProblemMapper.ToProblem(this, localizer, PolicyError.PolicyNotFound)
            : Ok(ResourceAssembler.ToResource(policy));
    }

    /// <summary>Registers a policy issued by an insurance company. The policy starts as <c>active</c>.</summary>
    /// <param name="resource">The policy data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created policy.</returns>
    /// <response code="201">The policy was registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The policyholder does not exist.</response>
    /// <response code="409">The policy number is already registered.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PolicyResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreatePolicy([FromBody] CreatePolicyResource resource, CancellationToken cancellationToken)
    {
        if (!EnumValueConverter.TryParse<InsuranceType>(resource.InsuranceType, out var insuranceType))
            return Problem(title: localizer["InvalidInsuranceType"].Value, statusCode: StatusCodes.Status400BadRequest);

        var command = new CreatePolicyCommand(resource.UserId, resource.ApplicationId, resource.PolicyNumber, insuranceType,
            resource.StartDate, resource.ExpirationDate, resource.PremiumAmount, resource.CoverageAmount);
        var result = await commandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Policy, PolicyError>.Success s => CreatedAtAction(nameof(GetPolicyById), new { policyId = s.Value.Id },
                ResourceAssembler.ToResource(s.Value)),
            Result<Policy, PolicyError>.Failure f => PolicyProblemMapper.ToProblem(this, localizer, f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}

/// <summary>
///     Maps policy errors to localized ProblemDetails responses.
/// </summary>
public static class PolicyProblemMapper
{
    /// <summary>Creates the ProblemDetails response for an error.</summary>
    /// <param name="controller">The controller that produces the response.</param>
    /// <param name="localizer">The string localizer.</param>
    /// <param name="error">The policy error.</param>
    /// <returns>The ProblemDetails response.</returns>
    public static ObjectResult ToProblem(ControllerBase controller, IStringLocalizer<SharedResource> localizer, PolicyError error) =>
        error switch
        {
            PolicyError.UserNotFound => controller.Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
            PolicyError.PolicyNotFound => controller.Problem(title: localizer["PolicyNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
            PolicyError.PolicyNumberAlreadyRegistered => controller.Problem(title: localizer["PolicyNumberAlreadyRegistered"].Value, statusCode: StatusCodes.Status409Conflict),
            PolicyError.PolicyNotRenewable => controller.Problem(title: localizer["PolicyNotRenewable"].Value, statusCode: StatusCodes.Status409Conflict),
            PolicyError.RenewalAlreadyRequested => controller.Problem(title: localizer["RenewalAlreadyRequested"].Value, statusCode: StatusCodes.Status409Conflict),
            _ => controller.Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
        };
}