using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the commands of insurance applications and their vehicle and life details.
/// </summary>
public interface IInsuranceApplicationCommandService
{
    /// <summary>Submits an insurance application.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created application or an error.</returns>
    Task<Result<InsuranceApplication, InsuranceApplicationError>> Handle(CreateInsuranceApplicationCommand command, CancellationToken cancellationToken = default);

    /// <summary>Deletes an insurance application and its details.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The deleted application or an error.</returns>
    Task<Result<InsuranceApplication, InsuranceApplicationError>> Handle(DeleteInsuranceApplicationCommand command, CancellationToken cancellationToken = default);

    /// <summary>Links an application with a vehicle.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created vehicle details or an error.</returns>
    Task<Result<VehicleInsuranceApplication, InsuranceApplicationError>> Handle(CreateVehicleInsuranceApplicationCommand command, CancellationToken cancellationToken = default);

    /// <summary>Marks an application as a life insurance application.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created life details or an error.</returns>
    Task<Result<LifeInsuranceApplication, InsuranceApplicationError>> Handle(CreateLifeInsuranceApplicationCommand command, CancellationToken cancellationToken = default);
}