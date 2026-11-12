namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Queries;

/// <summary>
///     Query to get indemnity requests associated with a beneficiary.
/// </summary>
public record GetIndemnityRequestsByBeneficiaryIdQuery(long BeneficiaryId);