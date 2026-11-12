using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Commands;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Vehicle details of an insurance application.
/// </summary>
public class VehicleInsuranceApplication
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected VehicleInsuranceApplication()
    {
    }

    /// <summary>
    ///     Links an insurance application with a vehicle.
    /// </summary>
    /// <param name="command">The create vehicle insurance application command.</param>
    public VehicleInsuranceApplication(CreateVehicleInsuranceApplicationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ApplicationId = command.ApplicationId;
        VehicleId = command.VehicleId;
    }

    /// <summary>Gets the identifier of the record.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the identifier of the insurance application.</summary>
    public long ApplicationId { get; private set; }

    /// <summary>Gets the identifier of the insured vehicle.</summary>
    public long VehicleId { get; private set; }
}