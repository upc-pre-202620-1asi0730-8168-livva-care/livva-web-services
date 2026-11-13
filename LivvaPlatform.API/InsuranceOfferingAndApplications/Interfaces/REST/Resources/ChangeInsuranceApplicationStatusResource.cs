using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Interfaces.REST.Resources;

/// <summary>
///     Data to change the status of an insurance application (for example, <c>{"status": "cancelled"}</c>).
/// </summary>
/// <param name="Status">The new status in lowercase.</param>
public record ChangeInsuranceApplicationStatusResource([Required(ErrorMessage = "RequiredStatus")] string Status);