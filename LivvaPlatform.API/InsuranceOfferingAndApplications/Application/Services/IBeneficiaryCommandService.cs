using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the commands of the Beneficiary aggregate.
/// </summary>
public interface IBeneficiaryCommandService
{
    /// <summary>Registers a beneficiary.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created beneficiary or an error.</returns>
    Task<Result<Beneficiary, BeneficiaryError>> Handle(CreateBeneficiaryCommand command, CancellationToken cancellationToken = default);

    /// <summary>Partially updates a beneficiary.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated beneficiary or an error.</returns>
    Task<Result<Beneficiary, BeneficiaryError>> Handle(UpdateBeneficiaryCommand command, CancellationToken cancellationToken = default);
}