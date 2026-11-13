using System.Net.Mime;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Errors;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Application.Services;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Queries;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.ValueObjects;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;
using LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Transform;
using LivvaPlatform.API.Resources;
using LivvaPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST;
/// <summary>
///     Maps insurance application errors to localized ProblemDetails responses.
/// </summary>
public static class InsuranceApplicationProblemMapper
{
    /// <summary>Creates the ProblemDetails response for an error.</summary>
    /// <param name="controller">The controller that produces the response.</param>
    /// <param name="localizer">The string localizer.</param>
    /// <param name="error">The application error.</param>
    /// <returns>The ProblemDetails response.</returns>
    public static ObjectResult ToProblem(ControllerBase controller, IStringLocalizer<SharedResource> localizer,
        InsuranceApplicationError error) => error switch
    {
        InsuranceApplicationError.UserNotFound => controller.Problem(title: localizer["UserNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        InsuranceApplicationError.ProductNotAvailable => controller.Problem(title: localizer["InsuranceProductNotAvailable"].Value, statusCode: StatusCodes.Status404NotFound),
        InsuranceApplicationError.ApplicationNotFound => controller.Problem(title: localizer["InsuranceApplicationNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        InsuranceApplicationError.VehicleNotFound => controller.Problem(title: localizer["VehicleNotFound"].Value, statusCode: StatusCodes.Status404NotFound),
        InsuranceApplicationError.ProductTypeMismatch => controller.Problem(title: localizer["ProductTypeMismatch"].Value, statusCode: StatusCodes.Status409Conflict),
        InsuranceApplicationError.ApplicationAlreadyLinked => controller.Problem(title: localizer["ApplicationAlreadyLinked"].Value, statusCode: StatusCodes.Status409Conflict),
        InsuranceApplicationError.InvalidStatusChange => controller.Problem(title: localizer["InvalidApplicationStatusChange"].Value, statusCode: StatusCodes.Status409Conflict),
        _ => controller.Problem(title: localizer["InvalidModelStateTitle"].Value, statusCode: StatusCodes.Status400BadRequest)
    };
}