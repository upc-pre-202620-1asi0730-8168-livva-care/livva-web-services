using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.PolicyManagement.Application.Errors;
using LivvaPlatform.API.PolicyManagement.Application.Services;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Commands;
using LivvaPlatform.API.PolicyManagement.Domain.Model.Queries;
using LivvaPlatform.API.PolicyManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.PolicyManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service that registers issued policies and handles renewal requests.
/// </summary>
/// <param name="policyRepository">Repository for accessing policies.</param>
/// <param name="renewalRepository">Repository for accessing renewals.</param>
/// <param name="iamContextFacade">Facade used to verify that the policyholder exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class PolicyCommandService(
    IPolicyRepository policyRepository,
    IRenewalRepository renewalRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<PolicyCommandService> logger) : IPolicyCommandService
{
    /// <inheritdoc />
    public async Task<Result<Policy, PolicyError>> Handle(CreatePolicyCommand command, CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<Policy, PolicyError>.Failure(PolicyError.UserNotFound);

        Policy policy;
        try
        {
            policy = new Policy(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while registering policy {PolicyNumber}", command.PolicyNumber);
            return new Result<Policy, PolicyError>.Failure(PolicyError.InvalidData);
        }

        if (await policyRepository.ExistsByPolicyNumberAsync(policy.PolicyNumber, cancellationToken))
            return new Result<Policy, PolicyError>.Failure(PolicyError.PolicyNumberAlreadyRegistered);

        await policyRepository.AddAsync(policy, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Policy, PolicyError>.Success(policy);
    }

    /// <inheritdoc />
    public async Task<Result<Renewal, PolicyError>> Handle(CreateRenewalCommand command, CancellationToken cancellationToken = default)
    {
        var policy = await policyRepository.FindByIdAsync(command.PolicyId, cancellationToken);
        if (policy is null) return new Result<Renewal, PolicyError>.Failure(PolicyError.PolicyNotFound);
        if (!policy.CanBeRenewed) return new Result<Renewal, PolicyError>.Failure(PolicyError.PolicyNotRenewable);

        var renewals = await renewalRepository.FindByPolicyIdAsync(policy.Id, cancellationToken);
        if (renewals.Any(r => r.IsOpen))
            return new Result<Renewal, PolicyError>.Failure(PolicyError.RenewalAlreadyRequested);

        var renewal = policy.RequestRenewal();
        await renewalRepository.AddAsync(renewal, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Renewal, PolicyError>.Success(renewal);
    }
}