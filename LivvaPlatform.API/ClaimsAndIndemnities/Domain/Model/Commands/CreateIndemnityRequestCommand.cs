namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;

/// <summary>
///     Command to register a life insurance indemnity request.
/// </summary>
/// <param name="PolicyId">The identifier of the insurance policy.</param>
/// <param name="BeneficiaryId">The identifier of the beneficiary requesting the indemnity.</param>
/// <param name="RequestDate">The date when the indemnity request was submitted.</param>
/// <param name="Description">The description of the indemnity request.</param>
/// <param name="IndemnityAmount">The requested indemnity amount, if specified.</param>
public record CreateIndemnityRequestCommand(
    long PolicyId,
    long BeneficiaryId,
    DateOnly RequestDate,
    string Description,
    decimal? IndemnityAmount);