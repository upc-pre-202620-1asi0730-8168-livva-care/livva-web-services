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
///     Queries the insurance products offered through Livva. Requires a valid access token.
/// </summary>
/// <param name="offeringQueryService">The offering query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Insurance Products")]
public class InsuranceProductsController(IOfferingQueryService offeringQueryService, IStringLocalizer<SharedResource> localizer)
    : ControllerBase
{
    /// <summary>Gets insurance products, optionally filtered by insurance type and insurer.</summary>
    /// <param name="insuranceType">Optional insurance type: <c>vehicle</c> or <c>life</c>.</param>
    /// <param name="insurerId">Optional identifier of the insurer.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The matching products (empty if none match).</returns>
    /// <response code="200">The matching products.</response>
    /// <response code="400">The insurance type is not valid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InsuranceProductResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetInsuranceProducts([FromQuery] string? insuranceType, [FromQuery] long? insurerId,
        CancellationToken cancellationToken)
    {
        InsuranceType? type = null;
        if (insuranceType is not null)
        {
            if (!EnumValueConverter.TryParse<InsuranceType>(insuranceType, out var parsed))
                return Problem(title: localizer["InvalidInsuranceType"].Value, statusCode: StatusCodes.Status400BadRequest);
            type = parsed;
        }

        var products = await offeringQueryService.Handle(new GetAllInsuranceProductsQuery(type, insurerId), cancellationToken);
        return Ok(products.Select(ResourceAssembler.ToResource));
    }

    /// <summary>Gets an insurance product by its identifier.</summary>
    /// <param name="productId">The identifier of the product.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The product.</returns>
    /// <response code="200">The product was found.</response>
    /// <response code="404">The product does not exist.</response>
    [HttpGet("{productId:long}")]
    [ProducesResponseType(typeof(InsuranceProductResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetInsuranceProductById(long productId, CancellationToken cancellationToken)
    {
        var product = await offeringQueryService.Handle(new GetInsuranceProductByIdQuery(productId), cancellationToken);
        return product is null
            ? Problem(title: localizer["InsuranceProductNotFound"].Value, statusCode: StatusCodes.Status404NotFound)
            : Ok(ResourceAssembler.ToResource(product));
    }
}