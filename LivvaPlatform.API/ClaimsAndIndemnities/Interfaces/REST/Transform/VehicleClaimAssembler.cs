using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;
using LivvaPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Transform;

/// <summary>
///     Converts vehicle claim resources to commands and aggregates to resources.
/// </summary>
public static class VehicleClaimAssembler
{
    /// <summary>Creates the command to report a vehicle claim.</summary>
    /// <param name="resource">The request data.</param>
    /// <param name="userId">The identifier of the insured user.</param>
    public static CreateVehicleClaimCommand ToCommand(CreateVehicleClaimResource resource, long userId) =>
        new(resource.PolicyId, userId, resource.IncidentDate, resource.IncidentLocation, resource.Description,
            resource.EstimatedDamageAmount);

    /// <summary>Creates the command to change the status of a vehicle claim.</summary>
    /// <returns>The command, or <c>null</c> when the status is not valid.</returns>
    public static ChangeVehicleClaimStatusCommand? ToCommand(long claimId, ChangeVehicleClaimStatusResource resource) =>
        EnumValueConverter.TryParse<VehicleClaimStatus>(resource.Status, out var status)
            ? new ChangeVehicleClaimStatusCommand(claimId, status, resource.Comment)
            : null;

    /// <summary>Converts a vehicle claim to its resource.</summary>
    public static VehicleClaimResource ToResource(VehicleClaim entity) =>
        new(entity.Id, entity.PolicyId, entity.UserId, entity.IncidentDate, entity.IncidentLocation,
            entity.Description, entity.EstimatedDamageAmount, EnumValueConverter.ToValue(entity.Status),
            entity.CompensationAmount, entity.ReportedAt, entity.ResolvedAt, entity.CreatedAt, entity.UpdatedAt);

    /// <summary>Converts a status history entry to its resource.</summary>
    public static ClaimStatusHistoryResource ToResource(ClaimStatusHistory entity) =>
        new(entity.Id, entity.ClaimId, EnumValueConverter.ToValue(entity.Status), entity.Comment, entity.ChangedAt,
            entity.ChangedAt);
}
