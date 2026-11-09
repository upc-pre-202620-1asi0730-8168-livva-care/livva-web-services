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
///     Application service for querying vehicles.
/// </summary>
/// <param name="vehicleRepository">Repository for accessing vehicles.</param>
public class VehicleQueryService(IVehicleRepository vehicleRepository) : IVehicleQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> Handle(GetAllVehiclesQuery query, CancellationToken cancellationToken = default) =>
        query.UserId is null
            ? await vehicleRepository.ListAsync(cancellationToken)
            : await vehicleRepository.FindByUserIdAsync(query.UserId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<Vehicle?> Handle(GetVehicleByIdQuery query, CancellationToken cancellationToken = default) =>
        await vehicleRepository.FindByIdAsync(query.VehicleId, cancellationToken);
}