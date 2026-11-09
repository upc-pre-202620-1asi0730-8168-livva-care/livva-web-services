using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the queries of insurers and insurance products.
/// </summary>
public interface IOfferingQueryService
{
    /// <summary>Gets all insurers.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The insurers.</returns>
    Task<IEnumerable<Insurer>> Handle(GetAllInsurersQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets an insurer by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The insurer if found; otherwise, null.</returns>
    Task<Insurer?> Handle(GetInsurerByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets insurance products, optionally filtered.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The products.</returns>
    Task<IEnumerable<InsuranceProduct>> Handle(GetAllInsuranceProductsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets an insurance product by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    Task<InsuranceProduct?> Handle(GetInsuranceProductByIdQuery query, CancellationToken cancellationToken = default);
}