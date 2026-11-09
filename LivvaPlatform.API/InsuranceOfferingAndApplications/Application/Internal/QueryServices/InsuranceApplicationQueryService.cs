using LivvaPlatform.API.IdentityAndProfileManagement.Interfaces.ACL;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;
using LivvaPlatform.API.Shared.Application.Patterns;
using LivvaPlatform.API.Shared.Domain.Repositories;
namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying insurance applications and their details.
/// </summary>
/// <param name="applicationRepository">Repository for accessing applications.</param>
/// <param name="vehicleApplicationRepository">Repository for accessing vehicle details.</param>
/// <param name="lifeApplicationRepository">Repository for accessing life details.</param>
public class InsuranceApplicationQueryService(
    IInsuranceApplicationRepository applicationRepository,
    IVehicleInsuranceApplicationRepository vehicleApplicationRepository,
    ILifeInsuranceApplicationRepository lifeApplicationRepository) : IInsuranceApplicationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<InsuranceApplication>> Handle(GetAllInsuranceApplicationsQuery query, CancellationToken cancellationToken = default) =>
        query.UserId is null
            ? await applicationRepository.ListAsync(cancellationToken)
            : await applicationRepository.FindByUserIdAsync(query.UserId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<InsuranceApplication?> Handle(GetInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default) =>
        await applicationRepository.FindByIdAsync(query.ApplicationId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleInsuranceApplication>> Handle(GetAllVehicleInsuranceApplicationsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.ApplicationId is null) return await vehicleApplicationRepository.ListAsync(cancellationToken);
        var details = await vehicleApplicationRepository.FindByApplicationIdAsync(query.ApplicationId.Value, cancellationToken);
        return details is null ? [] : [details];
    }

    /// <inheritdoc />
    public async Task<VehicleInsuranceApplication?> Handle(GetVehicleInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default) =>
        await vehicleApplicationRepository.FindByIdAsync(query.VehicleInsuranceApplicationId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<LifeInsuranceApplication>> Handle(GetAllLifeInsuranceApplicationsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.ApplicationId is null) return await lifeApplicationRepository.ListAsync(cancellationToken);
        var details = await lifeApplicationRepository.FindByApplicationIdAsync(query.ApplicationId.Value, cancellationToken);
        return details is null ? [] : [details];
    }

    /// <inheritdoc />
    public async Task<LifeInsuranceApplication?> Handle(GetLifeInsuranceApplicationByIdQuery query, CancellationToken cancellationToken = default) =>
        await lifeApplicationRepository.FindByIdAsync(query.LifeInsuranceApplicationId, cancellationToken);
}