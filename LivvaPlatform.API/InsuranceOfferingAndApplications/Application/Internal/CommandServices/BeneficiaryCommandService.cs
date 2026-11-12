using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.PolicyManagement.Interfaces.ACL;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.CommandServices;

/// <summary>
///     Application service that registers and updates beneficiaries.
/// </summary>
/// <param name="beneficiaryRepository">Repository for accessing beneficiaries.</param>
/// <param name="policyManagementContextFacade">Facade used to verify that the policy exists.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class BeneficiaryCommandService(
    IBeneficiaryRepository beneficiaryRepository,
    IPolicyManagementContextFacade policyManagementContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<BeneficiaryCommandService> logger) : IBeneficiaryCommandService
{
    /// <inheritdoc />
    public async Task<Result<Beneficiary, BeneficiaryError>> Handle(CreateBeneficiaryCommand command, CancellationToken cancellationToken = default)
    {
        if (!await policyManagementContextFacade.PolicyExistsAsync(command.PolicyId, cancellationToken))
            return new Result<Beneficiary, BeneficiaryError>.Failure(BeneficiaryError.PolicyNotFound);

        Beneficiary beneficiary;
        try
        {
            beneficiary = new Beneficiary(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while registering a beneficiary for policy {PolicyId}", command.PolicyId);
            return new Result<Beneficiary, BeneficiaryError>.Failure(BeneficiaryError.InvalidData);
        }

        await beneficiaryRepository.AddAsync(beneficiary, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Beneficiary, BeneficiaryError>.Success(beneficiary);
    }

    /// <inheritdoc />
    public async Task<Result<Beneficiary, BeneficiaryError>> Handle(UpdateBeneficiaryCommand command, CancellationToken cancellationToken = default)
    {
        var beneficiary = await beneficiaryRepository.FindByIdAsync(command.BeneficiaryId, cancellationToken);
        if (beneficiary is null) return new Result<Beneficiary, BeneficiaryError>.Failure(BeneficiaryError.BeneficiaryNotFound);

        try
        {
            beneficiary.UpdateInformation(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid data while updating beneficiary {BeneficiaryId}", command.BeneficiaryId);
            return new Result<Beneficiary, BeneficiaryError>.Failure(BeneficiaryError.InvalidData);
        }

        beneficiaryRepository.Update(beneficiary);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Beneficiary, BeneficiaryError>.Success(beneficiary);
    }
}