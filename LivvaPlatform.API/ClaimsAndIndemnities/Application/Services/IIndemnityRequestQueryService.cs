using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles queries for indemnity requests.
/// </summary>
public interface IIndemnityRequestQueryService
{
    /// <summary>Gets an indemnity request by its identifier.</summary>
    Task<IndemnityRequest?> Handle(
        GetIndemnityRequestByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets indemnity requests associated with a beneficiary.</summary>
    Task<IEnumerable<IndemnityRequest>> Handle(
        GetIndemnityRequestsByBeneficiaryIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets indemnity requests associated with a policy.</summary>
    Task<IEnumerable<IndemnityRequest>> Handle(
        GetIndemnityRequestsByPolicyIdQuery query,
        CancellationToken cancellationToken = default);
}