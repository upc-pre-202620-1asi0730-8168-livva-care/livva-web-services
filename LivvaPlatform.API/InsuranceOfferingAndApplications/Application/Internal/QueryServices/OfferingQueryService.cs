using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying insurers and insurance products.
/// </summary>
/// <param name="insurerRepository">Repository for accessing insurers.</param>
/// <param name="insuranceProductRepository">Repository for accessing insurance products.</param>
public class OfferingQueryService(
    IInsurerRepository insurerRepository,
    IInsuranceProductRepository insuranceProductRepository) : IOfferingQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Insurer>> Handle(GetAllInsurersQuery query, CancellationToken cancellationToken = default) =>
        await insurerRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Insurer?> Handle(GetInsurerByIdQuery query, CancellationToken cancellationToken = default) =>
        await insurerRepository.FindByIdAsync(query.InsurerId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<InsuranceProduct>> Handle(GetAllInsuranceProductsQuery query, CancellationToken cancellationToken = default) =>
        await insuranceProductRepository.FindByFiltersAsync(query.InsuranceType, query.InsurerId, cancellationToken);

    /// <inheritdoc />
    public async Task<InsuranceProduct?> Handle(GetInsuranceProductByIdQuery query, CancellationToken cancellationToken = default) =>
        await insuranceProductRepository.FindByIdAsync(query.ProductId, cancellationToken);
}