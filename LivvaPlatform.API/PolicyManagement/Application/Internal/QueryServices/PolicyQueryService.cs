using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Application.Services;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying policies, coverages, documents and renewals.
/// </summary>
/// <param name="policyRepository">Repository for accessing policies.</param>
/// <param name="coverageRepository">Repository for accessing coverages.</param>
/// <param name="documentRepository">Repository for accessing documents.</param>
/// <param name="renewalRepository">Repository for accessing renewals.</param>
public class PolicyQueryService(
    IPolicyRepository policyRepository,
    IPolicyCoverageRepository coverageRepository,
    IPolicyDocumentRepository documentRepository,
    IRenewalRepository renewalRepository) : IPolicyQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Policy>> Handle(GetPoliciesByUserIdQuery query, CancellationToken cancellationToken = default) =>
        await policyRepository.FindByUserIdAsync(query.UserId, cancellationToken);

    /// <inheritdoc />
    public async Task<Policy?> Handle(GetPolicyByIdQuery query, CancellationToken cancellationToken = default) =>
        await policyRepository.FindByIdAsync(query.PolicyId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<PolicyCoverage>> Handle(GetPolicyCoveragesQuery query, CancellationToken cancellationToken = default) =>
        query.PolicyId is null
            ? await coverageRepository.ListAsync(cancellationToken)
            : await coverageRepository.FindByPolicyIdAsync(query.PolicyId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<PolicyCoverage?> Handle(GetPolicyCoverageByIdQuery query, CancellationToken cancellationToken = default) =>
        await coverageRepository.FindByIdAsync(query.PolicyCoverageId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<PolicyDocument>> Handle(GetPolicyDocumentsQuery query, CancellationToken cancellationToken = default) =>
        query.PolicyId is null
            ? await documentRepository.ListAsync(cancellationToken)
            : await documentRepository.FindByPolicyIdAsync(query.PolicyId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<PolicyDocument?> Handle(GetPolicyDocumentByIdQuery query, CancellationToken cancellationToken = default) =>
        await documentRepository.FindByIdAsync(query.PolicyDocumentId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Renewal>> Handle(GetRenewalsQuery query, CancellationToken cancellationToken = default) =>
        query.PolicyId is null
            ? await renewalRepository.ListAsync(cancellationToken)
            : await renewalRepository.FindByPolicyIdAsync(query.PolicyId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<Renewal?> Handle(GetRenewalByIdQuery query, CancellationToken cancellationToken = default) =>
        await renewalRepository.FindByIdAsync(query.RenewalId, cancellationToken);
}