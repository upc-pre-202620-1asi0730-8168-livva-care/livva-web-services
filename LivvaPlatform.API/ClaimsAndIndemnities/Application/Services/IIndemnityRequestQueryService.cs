using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles queries for indemnity requests.
/// </summary>
public interface IIndemnityRequestQueryService
{
    /// <summary>Gets an indemnity request by its identifier.</summary>
    Task<Result<IndemnityRequest, IndemnityRequestError>> Handle(
        GetIndemnityRequestByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets indemnity requests associated with a beneficiary.</summary>
    Task<Result<IEnumerable<IndemnityRequest>, IndemnityRequestError>> Handle(
        GetIndemnityRequestsByBeneficiaryIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets indemnity requests associated with a policy.</summary>
    Task<Result<IEnumerable<IndemnityRequest>, IndemnityRequestError>> Handle(
        GetIndemnityRequestsByPolicyIdQuery query,
        CancellationToken cancellationToken = default);
}