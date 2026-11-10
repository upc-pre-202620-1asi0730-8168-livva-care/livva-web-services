using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Transform;

/// <summary>
///     Assembles commands and resources for vehicle claims.
/// </summary>
public static class VehicleClaimAssembler
{
    /// <summary>Converts a creation resource into a command.</summary>
    public static CreateVehicleClaimCommand ToCommand(
        CreateVehicleClaimResource resource) =>
        new(
            resource.PolicyId,
            resource.UserId,
            resource.ClaimNumber,
            resource.IncidentDate,
            resource.Description,
            resource.CompensationAmount);

    /// <summary>Converts a status resource into a command.</summary>
    public static ChangeVehicleClaimStatusCommand ToCommand(
        long claimId,
        ChangeVehicleClaimStatusResource resource) =>
        new(
            claimId,
            resource.Status,
            resource.Comment);

    /// <summary>Converts a vehicle claim into a resource.</summary>
    public static VehicleClaimResource ToResource(VehicleClaim entity) =>
        new(
            entity.Id,
            entity.PolicyId,
            entity.UserId,
            entity.ClaimNumber,
            entity.IncidentDate,
            entity.Description,
            entity.Status,
            entity.CompensationAmount,
            entity.ResolvedAt,
            entity.StatusHistories
                .Select(ToResource));

    /// <summary>Converts a claim status history into a resource.</summary>
    public static ClaimStatusHistoryResource ToResource(
        ClaimStatusHistory entity) =>
        new(
            entity.Id,
            entity.Status,
            entity.Comment,
            entity.ChangedAt);
}