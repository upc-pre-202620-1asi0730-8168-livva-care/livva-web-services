using LivvaPlatform.API.ClaimsAndIndemnities.Application.Errors;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.Shared.Application.Patterns;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Application.Services;

/// <summary>
///     Handles the commands of the IndemnityRequest aggregate.
/// </summary>
public interface IIndemnityRequestCommandService
{
    /// <summary>Registers an indemnity request.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created indemnity request or an error.</returns>
    Task<Result<IndemnityRequest, IndemnityRequestError>> Handle(
        CreateIndemnityRequestCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Changes the status of an indemnity request.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated indemnity request or an error.</returns>
    Task<Result<IndemnityRequest, IndemnityRequestError>> Handle(
        ChangeIndemnityRequestStatusCommand command,
        CancellationToken cancellationToken = default);
}