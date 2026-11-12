namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;

/// <summary>
///     Errors that can occur when managing indemnity requests.
/// </summary>
public enum IndemnityRequestError
{
    /// <summary>The related policy does not exist.</summary>
    PolicyNotFound,

    /// <summary>The beneficiary does not exist.</summary>
    BeneficiaryNotFound,

    /// <summary>The indemnity request does not exist.</summary>
    IndemnityRequestNotFound,

    /// <summary>The indemnity request cannot be updated to the requested status.</summary>
    InvalidStatusChange,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}