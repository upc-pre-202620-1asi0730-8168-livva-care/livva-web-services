using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;

/// <summary>
///     Command to change the status of a life insurance indemnity request.
/// </summary>
/// <param name="IndemnityRequestId">The identifier of the indemnity request.</param>
/// <param name="Status">The new status assigned to the indemnity request.</param>
/// <param name="Comment">The comment explaining the status change.</param>
public record ChangeIndemnityRequestStatusCommand(
    long IndemnityRequestId,
    IndemnityRequestStatus Status,
    string Comment);