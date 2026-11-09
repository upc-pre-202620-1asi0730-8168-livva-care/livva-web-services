using LivvaPlatform.API.PolicyManagement.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;

/// <summary>
///     Command to register a policy issued by an insurance company.
/// </summary>
public record CreatePolicyCommand(long UserId, long ApplicationId, string PolicyNumber, InsuranceType InsuranceType, DateOnly StartDate, DateOnly ExpirationDate, decimal PremiumAmount, decimal CoverageAmount);
