using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the queries of the Beneficiary aggregate.
/// </summary>
public interface IBeneficiaryQueryService
{
    /// <summary>Gets beneficiaries, optionally filtered by policy.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The beneficiaries.</returns>
    Task<IEnumerable<Beneficiary>> Handle(GetAllBeneficiariesQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a beneficiary by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The beneficiary if found; otherwise, null.</returns>
    Task<Beneficiary?> Handle(GetBeneficiaryByIdQuery query, CancellationToken cancellationToken = default);
}