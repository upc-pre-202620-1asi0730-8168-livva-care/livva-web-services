using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;

/// <summary>
///     Handles the queries of insurance applications and their vehicle and life details.
/// </summary>
public interface IInsuranceApplicationQueryService
{
    /// <summary>Gets applications, optionally filtered by applicant.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The applications.</returns>
    Task<IEnumerable<InsuranceApplication>> Handle(GetAllInsuranceApplicationsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets an application by its identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The application if found; otherwise, null.</returns>
    Task<InsuranceApplication?> Handle(GetInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle details, optionally filtered by application.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicle details.</returns>
    Task<IEnumerable<VehicleInsuranceApplication>> Handle(GetAllVehicleInsuranceApplicationsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets vehicle details by identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The vehicle details if found; otherwise, null.</returns>
    Task<VehicleInsuranceApplication?> Handle(GetVehicleInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets life details, optionally filtered by application.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The life details.</returns>
    Task<IEnumerable<LifeInsuranceApplication>> Handle(GetAllLifeInsuranceApplicationsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Gets life details by identifier.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The life details if found; otherwise, null.</returns>
    Task<LifeInsuranceApplication?> Handle(GetLifeInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default);
}