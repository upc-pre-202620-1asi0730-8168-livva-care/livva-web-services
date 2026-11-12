using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Life details of an insurance application.
/// </summary>
public class LifeInsuranceApplication
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected LifeInsuranceApplication()
    {
    }

    /// <summary>
    ///     Marks an insurance application as a life insurance application.
    /// </summary>
    /// <param name="command">The create life insurance application command.</param>
    public LifeInsuranceApplication(CreateLifeInsuranceApplicationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ApplicationId = command.ApplicationId;
    }

    /// <summary>Gets the identifier of the record.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the insurance application.</summary>
    public long ApplicationId { get; private set; }
}