namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;

/// <summary>
///     Represents an insurance company associated with Livva.
/// </summary>
/// <remarks>Insurers are loaded as reference data; the external reference is unique.</remarks>
public partial class Insurer
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Insurer()
    {
        ExternalReference = string.Empty;
        Name = string.Empty;
    }

    /// <summary>Gets the identifier of the insurer.</summary>
    public long Id { get; private set; }

    /// <summary>Gets the unique reference of the insurer in the external insurance company system.</summary>
    public string ExternalReference { get; private set; }

    /// <summary>Gets the name of the insurer.</summary>
    public string Name { get; private set; }

    /// <summary>Gets a value indicating whether the insurer is active.</summary>
    public bool Active { get; private set; }

    /// <summary>Activates the insurer.</summary>
    public void Activate() => Active = true;

    /// <summary>Deactivates the insurer.</summary>
    public void Deactivate() => Active = false;
}