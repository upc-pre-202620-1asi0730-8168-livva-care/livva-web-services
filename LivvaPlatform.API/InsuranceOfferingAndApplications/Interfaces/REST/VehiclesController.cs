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
///     Registers, queries, updates and deletes vehicles. Requires a valid access token.
/// </summary>
/// <param name="vehicleCommandService">The vehicle command service.</param>
/// <param name="vehicleQueryService">The vehicle query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Vehicles")]
public class VehiclesController(
    IVehicleCommandService vehicleCommandService,
    IVehicleQueryService vehicleQueryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>Registers a vehicle.</summary>
    /// <param name="resource">The vehicle data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created vehicle.</returns>
    /// <response code="201">The vehicle was registered.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The owner user does not exist.</response>
    /// <response code="409">The license plate is already registered.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateVehicle([FromBody] CreateVehicleResource resource, CancellationToken cancellationToken)
    {
        var result = await vehicleCommandService.Handle(ResourceAssembler.ToCommand(resource), cancellationToken);
        return result switch
        {
            Result<Vehicle, VehicleError>.Success s => CreatedAtAction(nameof(GetVehicleById), new { vehicleId = s.Value.Id },
                ResourceAssembler.ToResource(s.Value)),
            Result<Vehicle, VehicleError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Gets vehicles, optionally filtered by owner.</summary>
    /// <param name="userId">Optional identifier of the owner user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching vehicles (empty if none match).</returns>
    /// <response code="200">The matching vehicles.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVehicles([FromQuery] long? userId, CancellationToken cancellationToken)
    {
        var vehicles = await vehicleQueryService.Handle(new GetAllVehiclesQuery(userId), cancellationToken);
        return Ok(vehicles.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets a vehicle by its identifier.</summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicle.</returns>
    /// <response code="200">The vehicle was found.</response>
    /// <response code="404">The vehicle does not exist.</response>
    [HttpGet("{vehicleId:long}")]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVehicleById(long vehicleId, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleQueryService.Handle(new GetVehicleByIdQuery(vehicleId), cancellationToken);
        return vehicle is null ? ToProblem(VehicleError.VehicleNotFound) : Ok(ResourceAssembler.ToResource(vehicle));
    }

    /// <summary>Partially updates a vehicle.</summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="resource">The fields to update. Omitted fields keep their current value.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated vehicle.</returns>
    /// <response code="200">The vehicle was updated.</response>
    /// <response code="400">The request contains invalid values.</response>
    /// <response code="404">The vehicle does not exist.</response>
    /// <response code="409">The license plate is already registered.</response>
    [HttpPatch("{vehicleId:long}")]
    [ProducesResponseType(typeof(VehicleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateVehicle(long vehicleId, [FromBody] UpdateVehicleResource resource,
        CancellationToken cancellationToken)
    {
        var result = await vehicleCommandService.Handle(ResourceAssembler.ToCommand(vehicleId, resource), cancellationToken);
        return result switch
        {
            Result<Vehicle, VehicleError>.Success s => Ok(ResourceAssembler.ToResource(s.Value)),
            Result<Vehicle, VehicleError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>Deletes a vehicle that is not used by an insurance application.</summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The vehicle was deleted.</response>
    /// <response code="404">The vehicle does not exist.</response>
    /// <response code="409">The vehicle is used by an insurance application.</response>
    [HttpDelete("{vehicleId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteVehicle(long vehicleId, CancellationToken cancellationToken)
    {
        var result = await vehicleCommandService.Handle(new DeleteVehicleCommand(vehicleId), cancellationToken);
        return result switch
        {
            Result<Vehicle, VehicleError>.Success => NoContent(),
            Result<Vehicle, VehicleError>.Failure f => ToProblem(f.Error),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private ObjectResult ToProblem(VehicleError error) => error switch
    {
        VehicleError.UserNotFound => Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        VehicleError.VehicleNotFound => Problem(title: localizer["VehicleNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        VehicleError.LicensePlateAlreadyRegistered => Problem(title: localizer["LicensePlateAlreadyRegistered"].Value, statusCode: StatusCodes.Status409Conflict),
        VehicleError.VehicleInUse => Problem(title: localizer["VehicleInUse"].Value, statusCode: StatusCodes.Status409Conflict),
        _ => Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
    };
}