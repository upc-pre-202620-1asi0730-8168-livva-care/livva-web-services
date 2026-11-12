using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Repositories;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Internal.CommandServices;

/// <summary>
///     Application service that registers indemnity requests and handles status changes.
/// </summary>
/// <param name="indemnityRequestRepository">Repository for accessing indemnity requests.</param>
/// <param name="beneficiaryRepository">Repository for accessing beneficiaries.</param>
/// <param name="policyManagementContextFacade">Facade for verifying insurance policies.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class IndemnityRequestCommandService(
    IIndemnityRequestRepository indemnityRequestRepository,
    IBeneficiaryRepository beneficiaryRepository,
    IPolicyManagementContextFacade policyManagementContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<IndemnityRequestCommandService> logger) : IIndemnityRequestCommandService
{
    /// <inheritdoc />
    public async Task<Result<IndemnityRequest, IndemnityRequestError>> Handle(
        CreateIndemnityRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await policyManagementContextFacade.PolicyExistsAsync(command.PolicyId, cancellationToken))
            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.PolicyNotFound);

        var beneficiaries = await beneficiaryRepository.FindByPolicyIdAsync(command.PolicyId, cancellationToken);

        if (!beneficiaries.Any(beneficiary => beneficiary.Id == command.BeneficiaryId))
            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.BeneficiaryNotFound);

        IndemnityRequest indemnityRequest;

        try
        {
            indemnityRequest = new IndemnityRequest(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid data while registering indemnity request for policy {PolicyId}",
                command.PolicyId);

            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.InvalidData);
        }

        await indemnityRequestRepository.AddAsync(indemnityRequest, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<IndemnityRequest, IndemnityRequestError>.Success(indemnityRequest);
    }

    /// <inheritdoc />
    public async Task<Result<IndemnityRequest, IndemnityRequestError>> Handle(
        ChangeIndemnityRequestStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var indemnityRequest = await indemnityRequestRepository.FindByIdAsync(
            command.IndemnityRequestId,
            cancellationToken);

        if (indemnityRequest is null)
            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.IndemnityRequestNotFound);

        try
        {
            indemnityRequest.ChangeStatus(command.Status, command.Comment);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid status change for indemnity request {IndemnityRequestId}",
                command.IndemnityRequestId);

            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.InvalidStatusChange);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(
                ex,
                "Indemnity request {IndemnityRequestId} cannot change to status {Status}",
                command.IndemnityRequestId,
                command.Status);

            return new Result<IndemnityRequest, IndemnityRequestError>.Failure(
                IndemnityRequestError.InvalidStatusChange);
        }

        indemnityRequestRepository.Update(indemnityRequest);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<IndemnityRequest, IndemnityRequestError>.Success(indemnityRequest);
    }
}