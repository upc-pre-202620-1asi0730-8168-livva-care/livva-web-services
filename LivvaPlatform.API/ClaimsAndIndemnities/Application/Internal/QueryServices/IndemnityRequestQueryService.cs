using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying indemnity requests.
/// </summary>
/// <param name="indemnityRequestRepository">Repository for accessing indemnity requests.</param>
public class IndemnityRequestQueryService(
    IIndemnityRequestRepository indemnityRequestRepository) : IIndemnityRequestQueryService
{
    /// <inheritdoc />
    public async Task<IndemnityRequest?> Handle(
        GetIndemnityRequestByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await indemnityRequestRepository.FindByIdAsync(
            query.IndemnityRequestId,
            cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<IndemnityRequest>> Handle(
        GetIndemnityRequestsByBeneficiaryIdQuery query,
        CancellationToken cancellationToken = default) =>
        await indemnityRequestRepository.FindByBeneficiaryIdAsync(query.BeneficiaryId);

    /// <inheritdoc />
    public async Task<IEnumerable<IndemnityRequest>> Handle(
        GetIndemnityRequestsByPolicyIdQuery query,
        CancellationToken cancellationToken = default) =>
        await indemnityRequestRepository.FindByPolicyIdAsync(query.PolicyId);
}