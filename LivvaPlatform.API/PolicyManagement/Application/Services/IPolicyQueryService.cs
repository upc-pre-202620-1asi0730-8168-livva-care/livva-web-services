using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.PolicyManagement.Application.Services;

/// <summary>
///     Handles the queries of policies, coverages, documents and renewals.
/// </summary>
public interface IPolicyQueryService
{
    /// <summary>Gets the policies of a user.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The policies.</returns>
    Task<IEnumerable<Policy>> Handle(GetPoliciesByUserIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a policy by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The policy if found; otherwise, null.</returns>
    Task<Policy?> Handle(GetPolicyByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets policy coverages.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The coverages.</returns>
    Task<IEnumerable<PolicyCoverage>> Handle(GetPolicyCoveragesQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a policy coverage by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The coverage if found; otherwise, null.</returns>
    Task<PolicyCoverage?> Handle(GetPolicyCoverageByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets policy documents.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The documents.</returns>
    Task<IEnumerable<PolicyDocument>> Handle(GetPolicyDocumentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a policy document by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The document if found; otherwise, null.</returns>
    Task<PolicyDocument?> Handle(GetPolicyDocumentByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets renewals.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The renewals.</returns>
    Task<IEnumerable<Renewal>> Handle(GetRenewalsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets a renewal by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The renewal if found; otherwise, null.</returns>
    Task<Renewal?> Handle(GetRenewalByIdQuery query, CancellationToken cancellationToken = default);
}