namespace LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;

/// <summary>
///     Command to request the renewal of a policy.
/// </summary>
public record CreateRenewalCommand(long PolicyId);
