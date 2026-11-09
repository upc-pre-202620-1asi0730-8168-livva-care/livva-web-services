namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;

/// <summary>
///     Errors that can occur when managing beneficiaries.
/// </summary>
public enum BeneficiaryError
{
    /// <summary>The beneficiary does not exist.</summary>
    BeneficiaryNotFound,

    /// <summary>The provided data is not valid.</summary>
    InvalidData
}