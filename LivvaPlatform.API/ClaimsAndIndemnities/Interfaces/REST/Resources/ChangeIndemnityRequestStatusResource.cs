using System.ComponentModel.DataAnnotations;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.ClaimsAndIndemnities.Interfaces.REST.Resources;

/// <summary>
///     Data required to change the status of a life insurance indemnity request.
/// </summary>
public record ChangeIndemnityRequestStatusResource(
    [EnumDataType(typeof(IndemnityRequestStatus))]
    IndemnityRequestStatus Status,

    [Required(ErrorMessage = "RequiredStatusComment")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "InvalidStatusCommentLength")]
    string Comment);
    