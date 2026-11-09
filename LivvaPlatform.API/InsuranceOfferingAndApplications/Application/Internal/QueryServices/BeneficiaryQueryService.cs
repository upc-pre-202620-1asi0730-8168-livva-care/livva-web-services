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
///     Application service for querying beneficiaries.
/// </summary>
/// <param name="beneficiaryRepository">Repository for accessing beneficiaries.</param>
public class BeneficiaryQueryService(IBeneficiaryRepository beneficiaryRepository) : IBeneficiaryQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Beneficiary>> Handle(GetAllBeneficiariesQuery query, CancellationToken cancellationToken = default) =>
        query.PolicyId is null
            ? await beneficiaryRepository.ListAsync(cancellationToken)
            : await beneficiaryRepository.FindByPolicyIdAsync(query.PolicyId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<Beneficiary?> Handle(GetBeneficiaryByIdQuery query, CancellationToken cancellationToken = default) =>
        await beneficiaryRepository.FindByIdAsync(query.BeneficiaryId, cancellationToken);
}