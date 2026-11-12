using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST;

/// <summary>
///     Status history of vehicle claims. Entries are recorded automatically when a claim is reported or changes status.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/claim-status-histories")]
[Produces("application/json")]
[Tags("Claim Status Histories")]
public class ClaimStatusHistoriesController(IVehicleClaimQueryService queryService) : ControllerBase
{
    /// <summary>Gets the status history of a vehicle claim, newest first.</summary>
    /// <param name="claimId">The identifier of the vehicle claim.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <response code="200">The status history (empty if the claim has none).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimStatusHistoryResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByClaimId([FromQuery] long claimId, CancellationToken cancellationToken)
    {
        var histories = await queryService.Handle(new GetClaimStatusHistoriesByClaimIdQuery(claimId),
            cancellationToken);
        return Ok(histories.Select(VehicleClaimAssembler.ToResource));
    }
}
