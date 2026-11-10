
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Transform;

/// <summary>
///     Assembles commands and resources for indemnity requests.
/// </summary>
public static class IndemnityRequestAssembler
{
    /// <summary>
    ///     Converts an indemnity request creation resource into a command.
    /// </summary>
    public static CreateIndemnityRequestCommand ToCommand(
        CreateIndemnityRequestResource resource) =>
        new(
            resource.PolicyId,
            resource.BeneficiaryId,
            resource.RequestDate,
            resource.Description,
            resource.IndemnityAmount);

    /// <summary>
    ///     Converts a status change resource into a command.
    /// </summary>
    public static ChangeIndemnityRequestStatusCommand ToCommand(
        long indemnityRequestId,
        ChangeIndemnityRequestStatusResource resource) =>
        new(
            indemnityRequestId,
            resource.Status,
            resource.Comment);

    /// <summary>
    ///     Converts an indemnity request entity into a REST resource.
    /// </summary>
    public static IndemnityRequestResource ToResource(
        IndemnityRequest entity) =>
        new(
            entity.Id,
            entity.PolicyId,
            entity.BeneficiaryId,
            entity.RequestDate,
            entity.Description,
            entity.Status,
            entity.IndemnityAmount,
            entity.ResolvedAt,
            entity.StatusHistories.Select(ToResource));

    /// <summary>
    ///     Converts an indemnity status history entity into a REST resource.
    /// </summary>
    public static IndemnityStatusHistoryResource ToResource(
        IndemnityStatusHistory entity) =>
        new(
            entity.Id,
            entity.Status,
            entity.Comment,
            entity.ChangedAt);
}
