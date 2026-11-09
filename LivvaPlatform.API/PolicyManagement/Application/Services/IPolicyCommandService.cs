using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.PolicyManagement.Application.Services;

/// <summary>
///     Handles the commands of policies and renewals.
/// </summary>
public interface IPolicyCommandService
{
    /// <summary>Registers an issued policy.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created policy or an error.</returns>
    Task<Result<Policy, PolicyError>> Handle(CreatePolicyCommand command, CancellationToken cancellationToken = default);

    /// <summary>Requests the renewal of a policy.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created renewal or an error.</returns>
    Task<Result<Renewal, PolicyError>> Handle(CreateRenewalCommand command, CancellationToken cancellationToken = default);
}